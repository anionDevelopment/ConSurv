using ConSurvBackend.Core.Model.CameraProperties.SoundTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class CreateCameraDTO
    {
        public string Name { get; set; }
        public SoundType SoundType { get; set; }
        public VideoType VideoType { get; set; }
    }
}
