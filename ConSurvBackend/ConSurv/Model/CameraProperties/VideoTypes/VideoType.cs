using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;

namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes
{
    public abstract class VideoType
    {
        public abstract void Accept(IVideoTypeInterface visitor);
        public abstract T Accept<T>(IVideoTypeInterface<T> visitor);
    }
    public interface IVideoTypeInterface
    {
        void Handle(ONVIF oNVIF);
        void Handle(RTSPStream rTSPStream);
    }
    public interface IVideoTypeInterface<T>
    {
        T Handle(ONVIF oNVIF);
        T Handle(RTSPStream rTSPStream);
    }
}
