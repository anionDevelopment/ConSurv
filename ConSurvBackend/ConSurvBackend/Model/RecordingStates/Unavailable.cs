namespace ConSurvBackend.Core.Model.RecordingStates
{
    public class Unavailable : RecordingState
    {
        public override T Accept<T>(IRecordingStateVisitor<T> visitor) => visitor.Handle(this);

        public override void Accept(IRecordingStateVisitor visitor) => visitor.Handle(this);
    }
}
