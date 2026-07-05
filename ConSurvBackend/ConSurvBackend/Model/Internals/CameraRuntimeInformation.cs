using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;
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

        /// <summary>The FFmpeg process that captures preview screenshots from the media-hub.</summary>
        public ExternalProgramExecutor ScreenshotProcess { get; }

        /// <summary>The FFmpeg process that produces the HLS (m3u8) stream from the media-hub.</summary>
        public ExternalProgramExecutor M3U8Process { get; }

        /// <summary>The FFmpeg recording process, or <c>null</c> if the camera is not recording continuously.</summary>
        public ExternalProgramExecutor? RecordProcess { get; }

        /// <summary>
        /// Initializes a new <see cref="CameraRuntimeInformation"/>.
        /// </summary>
        public CameraRuntimeInformation(Camera camera, ushort port, string mediaMTXURL, ExternalProgramExecutor mediaMTXProcess, ExternalProgramExecutor streamToMediaMTXProcess, ExternalProgramExecutor screenshotProcess, ExternalProgramExecutor m3u8Process, ExternalProgramExecutor? recordProcess)
        {
            this.Camera = camera;
            this.Port = port;
            this.MediaMTXURL = mediaMTXURL;
            this.MediaMTXProcess = mediaMTXProcess;
            this.StreamToMediaMTXProcess = streamToMediaMTXProcess;
            this.ScreenshotProcess = screenshotProcess;
            this.M3U8Process = m3u8Process;
            this.RecordProcess = recordProcess;
        }

        /// <summary>
        /// Enumerates every process that was started for the camera (excluding optional processes that
        /// were never started, such as the recording process for a non-recording camera).
        /// </summary>
        /// <returns>All started processes belonging to the camera.</returns>
        public IEnumerable<ExternalProgramExecutor> GetAllProcesses()
        {
            yield return this.MediaMTXProcess;
            yield return this.StreamToMediaMTXProcess;
            yield return this.ScreenshotProcess;
            yield return this.M3U8Process;
            if (this.RecordProcess is not null)
            {
                yield return this.RecordProcess;
            }
        }
    }
}
