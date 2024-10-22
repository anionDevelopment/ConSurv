namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo
{
    public class ONVIF : VideoType
    {
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
