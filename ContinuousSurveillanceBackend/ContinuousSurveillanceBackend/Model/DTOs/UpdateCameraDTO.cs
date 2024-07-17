using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Model.DTOs
{
    public class UpdateCameraDTO
    {
        public string Name { get; set; }
        public string CameraAddress { get; set; }
        public RecordMode RecordMode { get; set; }
    }
}
