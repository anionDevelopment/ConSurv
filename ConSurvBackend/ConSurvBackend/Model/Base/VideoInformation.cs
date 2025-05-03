using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Model.Base
{
    public class VideoInformation
    {
        public string StreamURL { get; set; }
        public bool IsONVIFCamera { get; internal set; }//probably ptz command is meant here
        //TODO add possibility to blacken a polygon from the video

        public byte[] GetPreview(Camera camera, IRTSPManager rtspManager, uint? maximalHeight, uint? maximalWidth,IGRYLog log)
        {
            return rtspManager.GetPreview(camera,  maximalHeight,  maximalWidth,true,log).picture;
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
