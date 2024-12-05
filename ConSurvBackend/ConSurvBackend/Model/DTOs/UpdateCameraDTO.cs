using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class UpdateCameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoInformationDTO VideoInformation { get; set; }
        public RecordModeDTO RecordMode { get; set; }

        internal Camera ToCamera()
        {
            return new Camera(this.CameraId, this.Name)
            {
                Id = this.CameraId,
                Name = this.Name,
                VideoInformation = this.VideoInformation.ToVideoInformation(),
                RecordMode= this.RecordMode.ToRecordMode(),
            };
        }
    }
}
