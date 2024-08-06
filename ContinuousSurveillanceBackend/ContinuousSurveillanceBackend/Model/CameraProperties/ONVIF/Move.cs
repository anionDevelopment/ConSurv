using ContinuousSurveillanceBackend.Core.Model.MoveDirections;

namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF
{
    public  class Move : ONVIFCommand
    {
        public MoveDirection MoveDirection { get; set; }

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
