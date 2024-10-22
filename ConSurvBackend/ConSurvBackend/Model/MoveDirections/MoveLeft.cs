namespace ConSurvBackend.Core.Model.MoveDirections
{
    public class MoveLeft : MoveDirection
    {
        public override void Accept(IMoveDirectionVisitor visitor) => visitor.Handle(this);

        public override T Accept<T>(IMoveDirectionVisitor<T> visitor) => visitor.Handle(this);
    }
}
