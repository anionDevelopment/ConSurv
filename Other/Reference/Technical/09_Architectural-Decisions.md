# 9. Architectural Decisions

## Decision-board

| Decision-identifier | Date | Decision | Reason and notes |
| ------------------- | ---- | -------- | ---------------- |
| D001 | 2026-01-02 | ConSurv should be hostable on premises. | Base requirement for many users. |
| D002 | 2026-10-19 | RTSP-streams are ingested over TCP only; UDP is neither offered globally nor per camera. | TCP guarantees complete delivery, deterministic failure-detection and friendliness towards containers and firewalls - all of which are core requirements for a recording-oriented VMS. Supporting UDP additionally would turn UDP-side issues (defective HLS-segments, torn keyframes, silently stalling processes) into ConSurv-bugs, require a configuration-schema change plus a second failure-detection path, and would not gain any benefit for the use-case (ConSurv is the only consumer of each stream, sub-second latency is irrelevant for surveillance). UDP-only sources are expected to be fronted by a TCP-capable RTSP-server such as MediaMTX. See `openspec/specs/transport-protocol/spec.md` and `ConSurvBackend/Other/Reference/ReferenceContent/Articles/RtspTransport.md`. |
