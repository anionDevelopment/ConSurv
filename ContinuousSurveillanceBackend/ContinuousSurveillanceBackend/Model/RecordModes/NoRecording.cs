namespace ContinuousSurveillanceBackend.Core.Model.RecordingModes
{
    public class NoRecording : RecordMode
    {
        public override T Accept<T>(IRecordModeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public override void Accept(IRecordModeVisitor visitor)
        {
             visitor.Handle(this);
        }
    }
}
