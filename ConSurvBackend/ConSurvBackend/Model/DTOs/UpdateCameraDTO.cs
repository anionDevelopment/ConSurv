using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object that carries the fields a client wishes to update for an existing camera.
    /// </summary>
    public class UpdateCameraDTO
    {
        public string CameraId { get; set; }
        public string Name { get; set; }
        public VideoInformationDTO VideoInformationDTO { get; set; }
        public RecordModeDTO RecordModeDTO { get; set; }

        /// <summary>
        /// Maps the DTO fields to a fully initialised <see cref="Camera"/> domain object
        /// that can replace the currently stored camera configuration.
        /// </summary>
        /// <returns>A new <see cref="Camera"/> instance reflecting the requested update.</returns>
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
