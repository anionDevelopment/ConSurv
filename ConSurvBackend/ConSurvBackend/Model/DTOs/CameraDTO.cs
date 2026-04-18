namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object that exposes the full observable state of a camera to clients,
    /// combining its identity, video source, recording configuration, and current record state.
    /// </summary>
    public class CameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoInformationDTO VideoInformationDTO { get; set; }
        public RecordModeDTO RecordModeDTO { get; set; }
        public RecordStateDTO RecordStateDTO { get; set; }

    }
}
