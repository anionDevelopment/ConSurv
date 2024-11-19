namespace ConSurvBackend.Core.Model.RecordingStates
{
    public class CurrentlyRecording : RecordingState
    {
        public override T Accept<T>(IRecordingStateVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public override void Accept(IRecordingStateVisitor visitor)
        {
            visitor.Handle(this);
        }
    }
}
