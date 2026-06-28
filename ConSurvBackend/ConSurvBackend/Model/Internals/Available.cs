using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Model.Internals
{
    /// <summary>
    /// Represents the state in which a camera's backing processes (FFmpeg and MediaMTX) are
    /// running and the stream is reachable via <see cref="MediaMTXURL"/>.
    /// </summary>
    public class Available : CameraInternalsBase
    {
        public ExternalProgramExecutor FFMPEGProcess { get; private set; }
        public ExternalProgramExecutor MediaMTXProcess { get; private set; }
        public string MediaMTXURL { get; private set; }

        /// <summary>
        /// Initializes the <see cref="Available"/> state for a camera.
        /// </summary>
        /// <param name="camera">The camera whose processes are available.</param>
        /// <param name="ffmpegProcess">The running FFmpeg process executor.</param>
        /// <param name="mediaMTXProcess">The running MediaMTX process executor.</param>
        /// <param name="mediaMTXURL">The URL at which the MediaMTX stream can be consumed.</param>
        public Available(Camera camera, ExternalProgramExecutor ffmpegProcess, ExternalProgramExecutor mediaMTXProcess, string mediaMTXURL) : base(camera)
        {
            this.FFMPEGProcess = ffmpegProcess;
            this.MediaMTXProcess = mediaMTXProcess;
            this.MediaMTXURL = mediaMTXURL;
        }

        /// <inheritdoc />
        public override void Accept(ICameraInternalsBaseVisitor visitor)
        {
            visitor.Handle(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(ICameraInternalsBaseVisitor<T> visitor)
        {
           return visitor.Handle(this);
        }
    }
}
