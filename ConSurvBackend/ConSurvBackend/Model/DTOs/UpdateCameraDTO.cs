using ConSurvBackend.Core.Model.CameraProperties.SoundTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.RecordingModes;

namespace ConSurvBackend.Core.Model.DTOs
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
