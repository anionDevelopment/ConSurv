using ContinuousSurveillanceBackend.Core.Model.CameraProperties.SoundTypes;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes;

namespace ContinuousSurveillanceBackend.Core.Model.DTOs
{
    public class CreateCameraDTO
    {
        public string Name { get; set; }
        public SoundType SoundType { get; set; }
        public VideoType VideoType { get; set; }
    }
}
