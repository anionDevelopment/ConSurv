using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object for the video-source configuration of a camera,
    /// carrying only the fields that are safe to expose over the API.
    /// </summary>
    public class VideoInformationDTO
    {
        public string StreamURL { get; set; }
        public bool IsONVIFCamera { get;  set; }

        /// <summary>
        /// Converts this DTO back into a <see cref="VideoInformation"/> domain object.
        /// </summary>
        /// <returns>A new <see cref="VideoInformation"/> populated from this DTO's fields.</returns>
        internal VideoInformation ToVideoInformation()
        {
            return new VideoInformation()
            {
                StreamURL = this.StreamURL,
                IsONVIFCamera = this.IsONVIFCamera,
            };
        }
    }
}
