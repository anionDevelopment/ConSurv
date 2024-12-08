namespace ConSurvBackend.Core.Model.DTOs
{
    public class CameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoInformationDTO VideoInformationDTO { get; set; }
        public RecordModeDTO RecordModeDTO { get; set; }
        public RecordStateDTO RecordStateDTO { get; set; }

    }
}
