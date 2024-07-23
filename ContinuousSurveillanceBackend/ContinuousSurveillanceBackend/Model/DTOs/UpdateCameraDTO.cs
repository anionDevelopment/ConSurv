using ContinuousSurveillanceBackend.Core.Model.CameraProperties.SoundTypes;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;

namespace ContinuousSurveillanceBackend.Core.Model.DTOs
{
    public class UpdateCameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public SoundType SoundType { get; set; }
        public VideoType VideoType { get; set; }
        public RecordMode RecordMode { get; set; }
    }
}
