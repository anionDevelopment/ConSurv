using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Services
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
           return this._Persistence.CreateCamera( name, notRecording);
        }

        public void RemoveCamera(string cameraId)
        {
            this._Persistence.RemoveCamera(cameraId);
        }

        public void UpdateCamera(string cameraId, string name, RecordMode recordMode)
        {
            this._Persistence.UpdateCamera(cameraId, name, recordMode);
        }
    }
}
