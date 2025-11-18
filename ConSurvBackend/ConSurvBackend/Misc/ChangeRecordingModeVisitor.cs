using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;

namespace ConSurvBackend.Core.Misc
{
    internal class ChangeRecordingModeVisitor : IRecordModeVisitor
    {
        private readonly Camera _Camera;
        private readonly IRuntimeData _RuntimeData;
        public ChangeRecordingModeVisitor(Camera camera, IRuntimeData runtimeData)
        {
            this._Camera = camera;
            this._RuntimeData = runtimeData;
        }

        public void Handle(NoRecording noRecording)
        {
            //TODO use _RuntimeData to notify CameraManagementService about the changed state 
        }

        public void Handle(RecordAlways recordAlways)
        {
            //TODO use _RuntimeData to notify CameraManagementService about the changed state
        }

        public void Handle(RecordOnMovements recordOnMovements)
        {
            //TODO use _RuntimeData to notify CameraManagementService about the changed state
        }
    }
}
