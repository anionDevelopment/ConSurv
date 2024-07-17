using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public interface ICameraService
    {
        void CreateCamera(string name, string cameraAddress, NoRecording notRecording);
        void UpdateCamera(string name, string cameraAddress, RecordMode recordMode);
        void RemoveCamera(string cameraId);
    }
}
