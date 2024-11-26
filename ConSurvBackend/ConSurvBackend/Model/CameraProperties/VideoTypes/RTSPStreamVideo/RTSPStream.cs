using ConSurvBackend.Core.Model.DTOs;
using System;

namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo
{
    public class RTSPStream : VideoType
    {
        public string StreamURL { get; set; }
        public override void Accept(IVideoTypeVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IVideoTypeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public override byte[] GetPreview(Camera camera, IRTSPManager rtspManager)
        {
           return rtspManager.GetPreview(camera.Id, this.StreamURL);
        }

        public override void StartRecordingAsync( Camera camera, IRTSPManager rtspManager, string targetFolder, TimeSpan videoLength,bool timeInUTC)
        {
            rtspManager.StartRecordingAsync(camera.Id, this.StreamURL, targetFolder, videoLength, timeInUTC);
        }

        public override void StopRecording(Camera camera, IRTSPManager rtspManager)
        {
            rtspManager.StopRecording(camera.Id);
        }

        public override VideoTypeDTO ToDTO()
        {
            return new RTSPStreamDTO()
            {
                VideoType = this.GetType().Name,
                StreamURL = this.StreamURL,
            };
        }
    }
}
