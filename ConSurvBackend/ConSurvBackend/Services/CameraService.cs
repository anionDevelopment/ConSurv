using ConSurvBackend.Core.Model.RecordingModes;
using ConSurvBackend.Core.Miscellaneous;
using ConSurvBackend.Core.Model;
using System;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands;

namespace ConSurvBackend.Core.Services
{
    public class CameraService : ICameraService
    {
        private readonly IPersistence _Persistence;
        public CameraService(IPersistence persistence)
        {
            this._Persistence = persistence;
        }
        public string CreateCamera(string name, NoRecording notRecording)
        {
            return this._Persistence.CreateCamera(name, notRecording);
        }

        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand)
        {
            Camera camera = this.GetCameraById(cameraId);

            onvifCommand.Accept(new RunONVIFCommandVisitor(camera));
        }

        public void RemoveCamera(string cameraId)
        {
            this._Persistence.RemoveCamera(cameraId);
        }

        public void UpdateCamera(string cameraId, string name, RecordMode recordMode)
        {
            this._Persistence.UpdateCamera(cameraId, name, recordMode);
        }

        public Camera GetCameraById(string cameraId)
        {
            throw new NotImplementedException();
        }

        public double GetRateOfAvailableCameras()
        {
            throw new NotImplementedException();
        }

        public string Register(string adminUsername, string initialAdminPassword)
        {
            throw new NotImplementedException();
        }

        public bool UserWithNameExists(string adminUsername)
        {
            throw new NotImplementedException();
        }
    }
}
