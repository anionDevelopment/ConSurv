# Limits

## GPU-acceleration (NVENC)

ConSurv uses FFmpeg to process the RTSP-streams of the cameras. On startup it probes once whether the NVIDIA-NVENC-encoder is usable on the current machine. If it is, the video-decoding is offloaded to the GPU (`-hwaccel cuda`) and the video-encoding is done by `h264_nvenc` instead of the CPU-encoder `libx264`. If no usable GPU is found, everything runs on the CPU. Only NVIDIA-GPUs are supported.

### How many encode-sessions a camera needs

Each camera occupies exactly **one** NVENC encode-session, used by the process which burns the overlay and the timestamp into the stream and republishes it to the local media-hub.

The other FFmpeg-processes of a camera do not consume an encode-session:

| Process | GPU-usage |
|---|---|
| Stream to media-hub | One NVENC encode-session |
| Screenshots (previews) | NVDEC decoding only; the JPGs are encoded on the CPU |
| HLS-stream (`m3u8`) | None; the video-stream is copied without re-encoding |
| Recording | None; the streams are copied without re-encoding |

NVDEC (decoding) is not subject to a session-limit, so the screenshot-processes never exhaust any quota.

### The limit of 8 concurrent encode-sessions

NVIDIA restricts the number of concurrent NVENC encode-sessions on consumer-GPUs (in NVIDIA's terminology: "non-qualified GPUs", which includes the whole GeForce-line). The current limit is **8 concurrent sessions per system**. Quoting the [NVENC Application Note](https://docs.nvidia.com/video-technologies/video-codec-sdk/13.0/nvenc-application-note/index.html):

> On non-qualified GPUs, the number of concurrent encode sessions is limited to 8 per system. This limit of 8 concurrent sessions per system applies to the combined number of encoding sessions executed on all non-qualified cards present in the system.

Three consequences are worth knowing:

- The limit counts **per system, not per GPU**. Adding a second GeForce-card to the machine does not raise the quota.
- The limit counts **across all processes**, not only across ConSurv. Any other application on the same machine which uses NVENC reduces the amount of sessions left for ConSurv.
- Workstation- and data-center-GPUs (Quadro, RTX A-series, Tesla, L4, ...) have no such restriction. There the amount of concurrent sessions is only bounded by the throughput of the hardware.

This means that a machine with a GeForce-GPU can serve **at most 8 cameras with GPU-encoding**, and only when nothing else on that machine uses NVENC.

### Behaviour when the limit is reached

When the encode-session cannot be created, FFmpeg terminates immediately during startup with a message such as:

```
[h264_nvenc] No capable devices found
[h264_nvenc] OpenEncodeSessionEx failed: incompatible client key (21)
```

(The wording of the error is misleading: it indicates an exhausted session-quota, not an authentication- or memory-problem.)

ConSurv detects this and restarts the affected camera with the CPU-encoder. The event is written to the log as `Falling back to CPU-encoding for this camera.`. The camera then keeps working, just with a higher CPU-load. The fallback is remembered for the rest of the lifetime of the process, so that the camera does not repeatedly attempt to acquire a GPU-session it cannot get.

Because the cameras are started one after another, the first cameras get the available GPU-sessions and the remaining ones fall back to the CPU.

### What the fallback does not cover

The fallback only reacts to an FFmpeg-process which fails to start. It does not react to a GPU which is merely too slow: if the amount of pixels per second exceeds what the GPU can encode, the processes keep running but produce dropped frames instead of dying. The session-limit and the throughput of the GPU are two independent bounds; staying below 8 cameras does not by itself guarantee that the GPU can keep up with them.
