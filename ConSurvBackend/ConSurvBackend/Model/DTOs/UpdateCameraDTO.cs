using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class UpdateCameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoInformationDTO VideoInformationDTO { get; set; }
        public RecordModeDTO RecordModeDTO { get; set; }

        internal Camera ToCamera()
        {
            return new Camera(this.CameraId, this.Name)
            {
                Id = this.CameraId,
                Name = this.Name,
                VideoInformation = this.VideoInformationDTO.ToVideoInformation(),
                RecordMode= this.RecordModeDTO.ToRecordMode(),
            };
        }
    }
}
