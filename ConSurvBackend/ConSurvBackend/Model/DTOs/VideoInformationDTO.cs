using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class VideoInformationDTO
    {
        public string StreamURL { get; set; }
        public bool IsONVIFCamera { get;  set; }

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
