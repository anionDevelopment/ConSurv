using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Model.Internals
{
    public class Available : CameraInternalsBase
    {
        public ExternalProgramExecutor FFMPEGProcess { get; private set; }
        public ExternalProgramExecutor MediaMTXProcess { get; private set; }
        public string MediaMTXURL { get; private set; }

        public Available(Camera camera, ExternalProgramExecutor ffmpegProcess, ExternalProgramExecutor mediaMTXProcess, string mediaMTXURL) : base(camera)
        {
            this.FFMPEGProcess = ffmpegProcess;
            this.MediaMTXProcess = mediaMTXProcess;
            this.MediaMTXURL = mediaMTXURL;
        }

        public override void Accept(ICameraInternalsBaseVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ICameraInternalsBaseVisitor<T> visitor)
        {
           return visitor.Handle(this);
        }
    }
}
