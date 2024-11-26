using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.RecordModes;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class UpdateCameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoTypeDTO VideoType { get; set; }
        public RecordModeDTO RecordMode { get; set; }
    }
}
