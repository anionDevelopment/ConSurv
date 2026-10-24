# RTSP transport

ConSurv ingests every camera-stream via RTSP. All FFmpeg-processes spawned against a camera (the republisher to the local media-hub, the preview-screenshot process, the HLS-fragmenter and the recorder) are started with `-rtsp_transport tcp`. This article documents why TCP is the default and what the trade-offs against UDP are.

## TCP vs UDP

RFC 2326 defines two transports for the media-payload of an RTSP-session:

- **TCP (interleaved)** — control-connection and RTP/RTCP-data share the same TCP-connection (§ 10.12).
- **UDP** — RTSP only carries the control-channel and negotiates a separate pair of UDP-flows (RTP + RTCP) per stream.

The following table compares the two transports for the use-case of a server that permanently consumes RTSP-streams for recording, previewing and re-streaming.

| Property | With TCP | Without TCP (UDP) |
|---|---|---|
| Reliability | High: guaranteed delivery and ordering, no packet-loss. | Lower: UDP-packets may be dropped; the receiver has no retransmit-mechanism. |
| Latency | Marginally higher due to retransmit-buffers. For surveillance irrelevant. | Marginally lower. |
| Port-requirements | A single TCP-port (typically 554 or 8554); RTP and RTCP are interleaved through the same connection. | Multiple UDP-ports per stream (RTSP control + two RTP + two RTCP). For *n* cameras this multiplies accordingly. |
| Container / NAT behaviour | Robust: one outbound TCP-connection is unaffected by Docker-bridges, NAT or stateful firewalls. | Fragile: UDP-NAT-mappings time out, return-packets can be dropped and bridges may reorder datagrams. |
| Bandwidth-overhead | Roughly 5-8 % more because of TCP-headers, ACKs and occasional retransmits. | Lower, since there are no ACKs or retransmits. |
| Behaviour under bandwidth-saturation | TCP throttles; latency rises, but data arrives in full and recordings stay intact. | UDP ignores congestion; packets are silently discarded, producing broken frames, torn keyframes and defective MP4-segments. |
| Multicast | Not supported (point-to-point only). | Supported (one source serves many consumers). Irrelevant for ConSurv because the server is the only consumer. |
| Behaviour on network-glitches | Connection-breaks are detected immediately; FFmpeg exits and the camera-reconciliation in ConSurv restarts the process cleanly. | No connection-state means no clear failure-signal: FFmpeg can stall silently while the process still appears to be running, requiring additional watchdogs. |
| Troubleshooting | Easy: `ss`, `netstat`, Wireshark and `tcpdump` show the connection-state clearly. | Harder: UDP is stateless, which complicates diagnosis especially inside container-networks. |
| CPU-load | Slightly higher due to retransmit-buffer handling. Negligible for small numbers of streams. | Slightly lower. |
| Quality of the HLS-output | Stable, because FFmpeg receives a complete byte-stream. | Keyframe-loss produces unplayable or visually corrupted HLS-segments. |
| Industry default for recording | De-facto standard for VMS and recording-oriented systems. | Default of live-preview clients that favour latency over integrity. |
| Behaviour over Wi-Fi | Robust against typical wireless packet-loss. | Visible artefacts whenever the wireless link glitches. |
| Firewall / enterprise-networks | Passes through almost any firewall because only one port is required. | Often blocked, because firewalls dislike the dynamic UDP return-paths. |
| Compatibility with RTSP-sources | Supported by professional IP-cameras (Hikvision, Dahua, Axis, Reolink, Ubiquiti etc.) and by dedicated RTSP-servers such as MediaMTX, GStreamer `rtsp-sink` or Frigate. | Supported more broadly in principle, but some RTSP-sources - especially minimalistic broadcasters that only implement RTP-over-UDP output - do not offer a TCP-interleaved transport at all. |

## Design decision: ConSurv only supports TCP

ConSurv deliberately supports exactly one RTSP-transport - TCP - and offers neither a global switch nor a per-camera option to use UDP. The reasons:

- **UDP-drawbacks would become ConSurv's responsibility.** All the UDP-side issues from the table above (defective HLS-segments, torn keyframes, silently stalling processes, container/NAT fragility) would manifest as ConSurv-bugs ("recordings are broken", "preview freezes"), even though their root cause lies in the transport.
- **The UDP-advantages do not apply to this use-case.** Lower latency, multicast and minimal overhead only matter when a stream is distributed to many latency-sensitive consumers. ConSurv is the single consumer of each camera-stream and the sub-second latency-difference is irrelevant for surveillance.
- **A second transport would add real cost.** It would require a configuration-schema change (with migration), additional UI, documentation, tests and a second failure-detection path for processes that silently stall instead of exiting.
- **UDP-only sources have a clean work-around.** Sources that cannot speak TCP-interleaved can be placed behind a TCP-capable RTSP-server (such as MediaMTX) which pulls or receives a push from the limited source and re-publishes it. This isolates the UDP-problem to a dedicated component instead of spreading it across ConSurv.

Keeping ConSurv on a single, reliable transport-path is therefore the preferred trade-off over adding optionality that would weaken the core guarantee of correct recordings.

## Implications for the RTSP source

An RTSP-source used with ConSurv must support the TCP-interleaved transport. All common IP-cameras and standalone RTSP-servers do. Minimalistic broadcasters that only emit RTP-over-UDP will not work; FFmpeg reports this as:

```
method SETUP failed: 461 Unsupported transport
Protocol not supported
```

In that case the source itself must be replaced by - or placed behind - an RTSP-server that supports both transports (for example by letting a TCP-capable RTSP-server pull from, or receive a push from, the limited source and re-publish it to ConSurv).
