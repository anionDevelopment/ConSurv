namespace ContinuousSurveillanceBackend.Core.Model.RecordingModes
{
    public abstract class RecordMode
    {
        public abstract T Accept<T>(IRecordModeVisitor<T> visitor);
        public abstract void Accept(IRecordModeVisitor visitor);
    }
    public interface IRecordModeVisitor<T>
    {
        T Handle(NoRecording noRecording);
        T Handle(RecordAlways recordAlways);
    }
    public interface IRecordModeVisitor
    {
        void Handle(NoRecording noRecording);
        void Handle(RecordAlways recordAlways);
    }
}
