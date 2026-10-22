# Transport-Protocol Specification

## Purpose

Records which transport-protocol ConSurv uses to ingest camera-streams via RTSP. The decision is deliberate and
not derivable from the source-code of a single component: it must be respected by every process which consumes
an RTSP-stream and by every configuration-surface which exposes camera-settings to the operator.

## Requirements

### Requirement: RTSP-streams SHALL be ingested over TCP only

Every FFmpeg-process which ConSurv spawns against a camera's RTSP-URL SHALL request the TCP-interleaved
transport by passing `-rtsp_transport tcp`. UDP-transport SHALL NOT be used for ingestion.

#### Scenario: A new FFmpeg-process consumes an RTSP-stream

- **WHEN** a background-service, a reconciliation-loop or any other component starts an FFmpeg-process which reads
  from a camera's RTSP-URL
- **THEN** the invocation includes `-rtsp_transport tcp` before the `-i` argument

#### Scenario: An RTSP-source only supports UDP

- **WHEN** an operator wants to connect an RTSP-source which does not support the TCP-interleaved transport
- **THEN** the source is placed behind a TCP-capable RTSP-server (for example MediaMTX) which re-publishes the
  stream, and ConSurv is pointed at that re-publisher - ConSurv itself is not changed to accept UDP

### Requirement: No configuration option SHALL expose the transport-protocol

The camera-configuration SHALL NOT contain a per-camera or global switch to choose between TCP and UDP for the
RTSP-transport. The transport is a product-wide invariant and is not an operator-decision.

#### Scenario: A camera is created or edited

- **WHEN** an operator creates or edits a camera through the API or the web-UI
- **THEN** the camera-model does not offer a field for the RTSP-transport, and the backend does not accept such a
  field in the request-body

### Requirement: An RTSP-source which rejects TCP SHALL cause the camera to be marked as not available

When a camera's RTSP-source refuses the TCP-interleaved transport (typically visible as FFmpeg reporting
`461 Unsupported transport` followed by `Protocol not supported`), ConSurv SHALL treat the camera as not
available and SHALL NOT silently fall back to UDP.

#### Scenario: The RTSP-source rejects the TCP-transport

- **WHEN** FFmpeg returns a non-zero exit-code because the camera's RTSP-source does not support TCP
- **THEN** the camera is marked as not available in the runtime-state and the reconciliation-loop continues to
  retry; the retry still uses TCP, not UDP
