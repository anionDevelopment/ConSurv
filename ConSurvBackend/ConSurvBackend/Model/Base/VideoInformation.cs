using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;

namespace ConSurvBackend.Core.Model.Base
{
    public class VideoInformation
    {
        public string StreamURL { get; set; }
        public bool IsONVIFCamera { get; internal set; }//probably ptz command is meant here
        //TODO add possibility to blacken a polygon from the video

        public byte[] GetPreview(Camera camera, IRTSPManager rtspManager)
        {
            return rtspManager.GetPreview(camera).picture;
        }


        public VideoInformationDTO ToDTO()
        {
            VideoInformationDTO result = new VideoInformationDTO()
            {
                StreamURL = this.StreamURL,
                IsONVIFCamera=this.IsONVIFCamera,
            };
            return result;
        }
    }
}
