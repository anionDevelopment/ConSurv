using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Model.Internals
{
    /// <summary>
    /// Holds all currently relevant runtime information for a single camera: the allocated media-hub
    /// port, the internal RTSP URL and every process that was started for the camera (the MediaMTX
    /// media-hub, the FFmpeg process that feeds the hub, the screenshot- and HLS-processes and – if
    /// the camera records continuously – the recording process).
    /// <para/>
    /// The camera-management-service keeps one instance per camera so it can both diagnose the
    /// camera's health and reliably terminate every process it started.
    /// </summary>
    public class CameraRuntimeInformation
    {
        /// <summary>The camera (including the configuration) this information belongs to.</summary>
        public Camera Camera { get; }

        /// <summary>The TCP port the MediaMTX media-hub was started on.</summary>
        public ushort Port { get; }

        /// <summary>The internal RTSP URL under which the camera stream is provided by MediaMTX.</summary>
        public string MediaMTXURL { get; }

        /// <summary>The MediaMTX media-hub process.</summary>
        public ExternalProgramExecutor MediaMTXProcess { get; }

        /// <summary>The FFmpeg process that pushes the camera stream into the media-hub.</summary>
        public ExternalProgramExecutor StreamToMediaMTXProcess { get; }

        /// <summary>
        /// The moment at which the media-hub and the process which feeds it were started.
        /// </summary>
        /// <remarks>
        /// It tells how long the camera is starting already, which is what distinguishes a camera that only needs a
        /// moment more from one that never becomes available.
        /// </remarks>
        public DateTime MomentOfTheStart { get; }

        /// <summary>The FFmpeg process that captures preview screenshots from the media-hub, or <c>null</c> while the camera is still starting.</summary>
        public ExternalProgramExecutor? ScreenshotProcess { get; private set; }

        /// <summary>The FFmpeg process that produces the HLS (m3u8) stream from the media-hub, or <c>null</c> while the camera is still starting.</summary>
        public ExternalProgramExecutor? M3U8Process { get; private set; }

        /// <summary>The FFmpeg recording process, or <c>null</c> if the camera is not recording continuously or is still starting.</summary>
        public ExternalProgramExecutor? RecordProcess { get; private set; }

        /// <summary>
        /// Whether the processes which read from the media-hub were started already.
        /// </summary>
        /// <remarks>
        /// They can only be started once the media-hub really provides the stream, because an FFmpeg which reads
        /// from a path without a publisher terminates immediately.
        /// </remarks>
        public bool ProcessesWhichReadFromTheMediaHubWereStarted => this.ScreenshotProcess is not null;

        /// <summary>
        /// Initializes a new <see cref="CameraRuntimeInformation"/> with the processes which exist as soon as the
        /// camera is started. The processes which read from the media-hub are added later by
        /// <see cref="SetProcessesWhichReadFromTheMediaHub"/>.
        /// </summary>
        public CameraRuntimeInformation(Camera camera, ushort port, string mediaMTXURL, ExternalProgramExecutor mediaMTXProcess, ExternalProgramExecutor streamToMediaMTXProcess, DateTime momentOfTheStart)
        {
            this.Camera = camera;
            this.Port = port;
            this.MediaMTXURL = mediaMTXURL;
            this.MediaMTXProcess = mediaMTXProcess;
            this.StreamToMediaMTXProcess = streamToMediaMTXProcess;
            this.MomentOfTheStart = momentOfTheStart;
        }

        /// <summary>
        /// Remembers the processes which read from the media-hub, after they were started.
        /// </summary>
        /// <param name="screenshotProcess">The process which captures the preview-screenshots.</param>
        /// <param name="m3u8Process">The process which produces the HLS-stream.</param>
        /// <param name="recordProcess">The recording process, or <c>null</c> if the camera does not record continuously.</param>
        public void SetProcessesWhichReadFromTheMediaHub(ExternalProgramExecutor screenshotProcess, ExternalProgramExecutor m3u8Process, ExternalProgramExecutor? recordProcess)
        {
            this.ScreenshotProcess = screenshotProcess;
            this.M3U8Process = m3u8Process;
            this.RecordProcess = recordProcess;
        }

        /// <summary>
        /// Enumerates every process that was started for the camera (excluding processes which were not started,
        /// such as the recording process for a non-recording camera or the processes which read from the media-hub
        /// while the camera is still starting).
        /// </summary>
        /// <returns>All started processes belonging to the camera.</returns>
        public IEnumerable<ExternalProgramExecutor> GetAllProcesses()
        {
            yield return this.MediaMTXProcess;
            yield return this.StreamToMediaMTXProcess;
            if (this.ScreenshotProcess is not null)
            {
                yield return this.ScreenshotProcess;
            }
            if (this.M3U8Process is not null)
            {
                yield return this.M3U8Process;
            }
            if (this.RecordProcess is not null)
            {
                yield return this.RecordProcess;
            }
        }
    }
}
