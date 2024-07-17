namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes
{
    public class RTSPStream: VideoType
    {
        public string StreamURL { get; set; }
        public override void Accept(IVideoTypeInterface visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IVideoTypeInterface<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
