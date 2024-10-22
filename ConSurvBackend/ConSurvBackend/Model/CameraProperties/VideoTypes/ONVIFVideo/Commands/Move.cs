using ConSurvBackend.Core.Model.MoveDirections;

namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands
{
    public class Move : ONVIFCommand
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
