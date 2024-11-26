namespace ConSurvBackend.Core.Model.DTOs
{
    public class CameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoTypeDTO VideoType { get; set; }
        public RecordModeDTO RecordMode { get; set; }
        public RecordingStateDTO CurrentRecordingState { get; set; }

    }
}
