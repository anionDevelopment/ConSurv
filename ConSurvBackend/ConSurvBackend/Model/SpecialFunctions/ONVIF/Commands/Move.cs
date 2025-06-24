using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.MoveDirections;

namespace ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands
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
