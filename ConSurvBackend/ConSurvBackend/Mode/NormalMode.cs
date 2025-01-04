namespace ConSurvBackend.Core.Mode
{
    internal class NormalMode:BaseMode
    {
        #region Overhead
        public override void Accept(IBaseModeVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IBaseModeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        #endregion
    }
}
