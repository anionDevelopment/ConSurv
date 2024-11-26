using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using ConSurvBackend.Core.Model.DTOs;
using System;

namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes
{
    public abstract class VideoType
    {
        /// <remarks>
        /// The implementing method is responsible to record until the stop-record-function will be called and it is also responsible to consider the <paramref name="videoLength"/> internally.
        /// </remarks>
        public abstract void StartRecordingAsync(Camera camera, IRTSPManager rtspManager, string targetFolder, TimeSpan videoLength, bool filenameInUTC);
        public abstract void StopRecording(Camera camera, IRTSPManager rtspManager);
        public abstract byte[] GetPreview(Camera camera, IRTSPManager rtspManager);
        public abstract void Accept(IVideoTypeVisitor visitor);
        public abstract T Accept<T>(IVideoTypeVisitor<T> visitor);
        public abstract VideoTypeDTO ToDTO();
    }
    public interface IVideoTypeVisitor
    {
        void Handle(RTSPStream rtspStream);
    }
    public interface IVideoTypeVisitor<T>
    {
        T Handle(RTSPStream rtspStream);
    }
}
