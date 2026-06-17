namespace ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.MoveDirections
{
    public class MoveLeft : MoveDirection
    {
        public override void Accept(IMoveDirectionVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IMoveDirectionVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
