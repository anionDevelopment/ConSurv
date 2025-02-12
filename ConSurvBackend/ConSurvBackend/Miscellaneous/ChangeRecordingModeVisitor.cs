using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal class ChangeRecordingModeVisitor : IRecordModeVisitor
    {
        private readonly Camera _Camera;
        private readonly IRTSPManager _RTSPManager;

        public ChangeRecordingModeVisitor(Camera camera, IRTSPManager rtspManager)
        {
            this._Camera = camera;
            this._RTSPManager = rtspManager;
        }

        public void Handle(NoRecording noRecording)
        {
            this._RTSPManager.EnsureNotRecording(this._Camera);
        }

        public void Handle(RecordAlways recordAlways)
        {
            this._RTSPManager.EnsureRecordingAlwaysAsync(this._Camera);
        }

        public void Handle(RecordOnMovements recordOnMovements)
        {
            this._RTSPManager.EnsureRecordingOnMovementsAsync(this._Camera);
        }
    }
}
