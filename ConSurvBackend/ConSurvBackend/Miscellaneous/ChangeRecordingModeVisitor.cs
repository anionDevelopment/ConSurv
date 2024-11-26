using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal class ChangeRecordingModeVisitor : IRecordModeVisitor
    {
        private readonly Camera _Camera;
        private readonly IGRYLog _Log;
        private readonly CodeUnitSpecificConfiguration _CodeUnitSpecificConfiguration;
        private readonly IRTSPManager _RTSPManager;

        public ChangeRecordingModeVisitor(Camera camera, IRTSPManager rtspManager, IGRYLog log, CodeUnitSpecificConfiguration codeUnitSpecificConfiguration)
        {
            this._Camera = camera;
            this._RTSPManager = rtspManager;
            this._Log = log;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
        }

        public void Handle(NoRecording noRecording/*new recording mode*/)
        {
            this._Camera.VideoType.Accept(new StopRecordingVisitor(this._Camera, this._RTSPManager, this._Log));
        }

        public void Handle(RecordAlways recordAlways/*new recording mode*/)
        {
            if (this._Camera.VideoType == null)
            {
                throw new BadRequestException($"Can not start recording because the videotype is undefined.");
            }
            else
            {
                this._Camera.VideoType.Accept(new StartRecordingVisitor(this._Camera, this._RTSPManager, this._Log, this._CodeUnitSpecificConfiguration));
            }
        }
    }
}
