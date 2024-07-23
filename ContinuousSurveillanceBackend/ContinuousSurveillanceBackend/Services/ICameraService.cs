using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public interface ICameraService
    {
        /// <returns>Returns the id of the created camera</returns>
        string CreateCamera(string name, NoRecording notRecording);
        void UpdateCamera(string cameraId, string name, RecordMode recordMode);
        void RemoveCamera(string cameraId);
    }
}
