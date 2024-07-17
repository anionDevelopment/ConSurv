using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using System;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public class CameraService : ICameraService
    {
        public void CreateCamera(string name, string cameraAddress, NoRecording notRecording)
        {
            throw new NotImplementedException();
        }

        public void RemoveCamera(string cameraId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCamera(string name, string cameraAddress, RecordMode recordMode)
        {
            throw new NotImplementedException();
        }
    }
}
