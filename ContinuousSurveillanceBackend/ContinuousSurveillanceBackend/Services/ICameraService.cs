using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public interface ICameraService
    {
        /// <returns>Returns the id of the created camera</returns>
        string CreateCamera(string name, NoRecording notRecording);
        void UpdateCamera(string cameraId, string name, RecordMode recordMode);
        void RemoveCamera(string cameraId);
        void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand);
        Camera GetCameraById(string cameraId);
    }
}
