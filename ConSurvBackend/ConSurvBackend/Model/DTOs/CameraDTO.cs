namespace ConSurvBackend.Core.Model.DTOs
{
    public class CameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoInformationDTO VideoInformation { get; set; }
        public RecordModeDTO RecordMode { get; set; }
        public RecordStateDTO RecordState { get; set; }

    }
}
