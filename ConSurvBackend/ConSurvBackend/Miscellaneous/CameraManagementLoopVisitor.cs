using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.RecordingModes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal class CameraManagementLoopVisitor : IRecordModeVisitor
    {
        private readonly IGeneralLogger _Logger;
        private readonly Camera _Camera;
        private readonly CodeUnitSpecificConfiguration _CodeUnitSpecificConfiguration;
        public CameraManagementLoopVisitor(IGeneralLogger logger,Camera camera, CodeUnitSpecificConfiguration codeUnitSpecificConfiguration)
        {
            this._Logger = logger;
            this._Camera = camera;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
        }

        public void Handle(NoRecording noRecording)
        {
            this._Camera.GetCurrentRecordingInformation().Accept(new CameraManagementLoopNoRecordingVisitor(this._Logger, this._Camera));
        }

        public void Handle(RecordAlways recordAlways)
        {
            this._Camera.GetCurrentRecordingInformation().Accept(new CameraManagementLoopRecordAlwaysVisitor(this._Logger, this._Camera, this._CodeUnitSpecificConfiguration));
        }
    }
}
