using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ContinuousSurveillanceBackend.Core.Miscellaneous
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

        public void Handle(Idle idle)
        {
            GUtilities.NoOperation();
        }

        public void Handle(Unavailable unavailable)
        {
            GUtilities.NoOperation();
        }
    }
}
