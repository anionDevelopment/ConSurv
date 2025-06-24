using ConSurvBackend.Core.BackgroundServices;
using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Model.Base
{
    public class VideoInformation
    {
        public string StreamURL { get; set; }
        public string? Certificate { get; set; }
        public bool IsONVIFCamera { get; internal set; }//probably ptz command is meant here
        //TODO add possibility to blacken a polygon from the video

        public byte[] GetPreview(Camera camera, IPreviewService previewService, uint? maximalHeight, uint? maximalWidth, IGRYLog log)
        {
            return previewService.GetPreview(camera.Id);
        }

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
