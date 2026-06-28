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
        public bool SupportsPTZViaONVIF { get;  set; }
        public string? ONVIFUrl { get; set; }
        public string? ONVIFUsername { get; set; }
        public string? ONVIFPassword { get; set; }

        /// <summary>
        /// Converts this DTO back into a <see cref="VideoInformation"/> domain object.
        /// </summary>
        /// <returns>A new <see cref="VideoInformation"/> populated from this DTO's fields.</returns>
        internal VideoInformation ToVideoInformation()
        {
            return new VideoInformation()
            {
                StreamURL = this.StreamURL,
                SupportsPTZViaONVIF = this.SupportsPTZViaONVIF,
                ONVIFUrl = this.ONVIFUrl,
                ONVIFUsername = this.ONVIFUsername,
                ONVIFPassword = this.ONVIFPassword,
            };
        }
    }
}
