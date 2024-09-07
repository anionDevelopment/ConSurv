namespace ContinuousSurveillanceBackend.Core.Model.MoveDirections
{
    public class MoveUp : MoveDirection
    {
        public override void Accept(IMoveDirectionVisitor visitor) => visitor.Handle(this);

        public override T Accept<T>(IMoveDirectionVisitor<T> visitor) => visitor.Handle(this);
    }
}
