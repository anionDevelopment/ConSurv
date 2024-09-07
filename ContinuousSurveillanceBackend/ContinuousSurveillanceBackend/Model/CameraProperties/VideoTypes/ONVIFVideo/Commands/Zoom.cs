using ContinuousSurveillanceBackend.Core.Model.ZoomDirections;

namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands
{
    public class Zoom : ONVIFCommand
    {
        public ZoomDirection ZoomDirection { get; set; }

        public override void Accept(IONVIFCommandVisitor visitor) => visitor.Handle(this);

        public override T Accept<T>(IONVIFCommandVisitor<T> visitor) => visitor.Handle(this);
    }
}
