using ConSurvBackend.Core.Model.DTOs;

namespace ConSurvBackend.Core.Model.Base
{
    /// <summary>
    /// Stores the video-source configuration for a camera, including the stream URL,
    /// an optional TLS certificate, and whether ONVIF PTZ control is available.
    /// </summary>
    public class VideoInformation
    {
        public string StreamURL { get; set; }
        public string? Certificate { get; set; }
        public bool IsONVIFCamera { get; internal set; }//probably ptz command is meant here

        /// <summary>
        /// Projects this instance into a <see cref="VideoInformationDTO"/> for client transfer.
        /// Sensitive fields (e.g. <see cref="Certificate"/>) are intentionally excluded from the DTO.
        /// </summary>
        /// <returns>A DTO containing the publicly shareable video information.</returns>
        public VideoInformationDTO ToDTO()
        {
            VideoInformationDTO result = new VideoInformationDTO()
            {
                StreamURL = this.StreamURL,
                IsONVIFCamera = this.IsONVIFCamera,
            };
            return result;
        }
    }
}
