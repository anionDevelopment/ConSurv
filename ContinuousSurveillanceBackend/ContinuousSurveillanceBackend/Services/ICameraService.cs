using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public interface ICameraService
    {
        void CreateCamera(string name, NoRecording notRecording);
        void UpdateCamera(string name, RecordMode recordMode);
        void RemoveCamera(string cameraId);
    }
}
