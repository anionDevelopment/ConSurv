using ConSurvBackend.Core.Model.DTOs;

namespace ConSurvBackend.Core.Model.Base
{
    public class VideoInformation
    {
        public string StreamURL { get; set; }
        public string? Certificate { get; set; }
        public bool IsONVIFCamera { get; internal set; }//probably ptz command is meant here

       
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
