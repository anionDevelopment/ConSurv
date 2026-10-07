using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Model.Internals
{
    /// <summary>
    /// Represents the state in which the media-hub of a camera and the process which feeds it are running, but the
    /// hub does not provide the stream yet.
    /// </summary>
    /// <remarks>
    /// This state exists because a camera which is still coming up must not be treated like a broken one. Feeding
    /// the hub takes several seconds (the stream has to be opened, analyzed, encoded and published), and without
    /// this state the camera would be reported as <see cref="NotAvailable"/> in that time, which terminates every
    /// process of it and starts the whole attempt again - so a camera which only needs a moment would never become
    /// available at all.
    /// </remarks>
    public class Starting : CameraInternalsBase
    {
        public ExternalProgramExecutor FFMPEGProcess { get; private set; }
        public ExternalProgramExecutor MediaMTXProcess { get; private set; }
        public string MediaMTXURL { get; private set; }

        /// <summary>
        /// Initializes the <see cref="Starting"/> state for a camera.
        /// </summary>
        /// <param name="camera">The camera which is starting.</param>
        /// <param name="ffmpegProcess">The running FFmpeg process which feeds the media-hub.</param>
        /// <param name="mediaMTXProcess">The running MediaMTX process.</param>
        /// <param name="mediaMTXURL">The URL under which the stream will be provided once the hub has it.</param>
        public Starting(Camera camera, ExternalProgramExecutor ffmpegProcess, ExternalProgramExecutor mediaMTXProcess, string mediaMTXURL) : base(camera)
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
