using ContinuousSurveillanceBackend.Core.Miscellaneous;
using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using System;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public class CameraService : ICameraService
    {
        public void CreateCamera(string name, NoRecording notRecording)
        {
            throw new NotImplementedException();
        }

        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand)
        {
            onvifCommand.Accept(new RunONVIFCommandVisitor(GetCameraById(cameraId)));
        }

        public void RemoveCamera(string cameraId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCamera(string name, RecordMode recordMode)
        {
            throw new NotImplementedException();
        }

        public Camera GetCameraById(string cameraId)
        {
            throw new NotImplementedException();
        }
    }
}
