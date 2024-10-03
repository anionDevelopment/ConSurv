using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Miscellaneous
{
    public class CameraManagementLoopNoRecordingVisitor : IRecordingStateVisitor
    {
        private readonly IGeneralLogger _Logger;
        private readonly Camera _Camera;

        public CameraManagementLoopNoRecordingVisitor(IGeneralLogger logger, Camera camera)
        {
            this._Logger = logger;
            this._Camera = camera;
        }

        public void Handle(CurrentlyRecording currentlyRecording)
        {
            System.Diagnostics.Process recordingProcess = currentlyRecording.GetRecordingProcess();
            throw new NotImplementedException();//TODO stop camera
        }

        public void Handle(Idle idle) => GUtilities.NoOperation();

        public void Handle(Unavailable unavailable) => GUtilities.NoOperation();
    }
}
