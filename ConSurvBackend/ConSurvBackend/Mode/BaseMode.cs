namespace ConSurvBackend.Core.Mode
{
    internal abstract class BaseMode
    {
        public abstract void Accept(IBaseModeVisitor visitor);
        public abstract T Accept<T>(IBaseModeVisitor<T> visitor);
    }
    internal interface IBaseModeVisitor
    {
        void Handle(ManualMode manualMode);
        void Handle(NormalMode normalMode);
    }
    internal interface IBaseModeVisitor<T>
    {
        T Handle(ManualMode manualMode);
        T Handle(NormalMode normalMode);
    }
}
