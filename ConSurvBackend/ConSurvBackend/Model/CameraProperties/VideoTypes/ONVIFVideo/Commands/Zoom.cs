using ConSurvBackend.Core.Model.ZoomDirections;

namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands
{
    public class Zoom : ONVIFCommand
    {
        public ZoomDirection ZoomDirection { get; set; }

        public override void Accept(IONVIFCommandVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IONVIFCommandVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
