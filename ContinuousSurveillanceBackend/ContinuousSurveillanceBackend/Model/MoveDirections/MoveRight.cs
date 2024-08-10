namespace ContinuousSurveillanceBackend.Core.Model.MoveDirections
{
    public class MoveRight : MoveDirection
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
