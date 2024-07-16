using ContinuousSurveillanceBackend.Core.Model;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public interface ICameraService
    {
        void CreateCamera(string name, string cameraAddress, NotRecording notRecording);
        void UpdateCamera(string name, string cameraAddress, RecordMode recordMode);
        void RemoveCamera(string cameraId);
    }
}
