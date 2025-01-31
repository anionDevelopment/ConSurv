using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;
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
            this._RTSPManager.EnsureNotRecording(this._Camera);
        }

        public void Handle(RecordAlways recordAlways/*new recording mode*/)
        {
            this._RTSPManager.EnsureRecordingAsync(this._Camera, this._CodeUnitSpecificConfiguration.TargetFolder, this._CodeUnitSpecificConfiguration.VideoLength, this._CodeUnitSpecificConfiguration.TimeInUTC);
        }

        public void Handle(RecordOnMovements recordOnMovements)
        {
            this._RTSPManager.EnsureRecordingOnMovementsAsync(this._Camera, this._CodeUnitSpecificConfiguration.TargetFolder, this._CodeUnitSpecificConfiguration.VideoLength, this._CodeUnitSpecificConfiguration.TimeInUTC);
        }
    }
}
