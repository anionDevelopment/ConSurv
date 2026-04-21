namespace ConSurvBackend.Core.Model.RecordStates
{
    /// <summary>
    /// Represents the state in which the camera is actively writing footage to disk.
    /// </summary>
    public class CurrentlyRecording : RecordState
    {
        /// <inheritdoc />
        public override T Accept<T>(IRecordStateVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        /// <inheritdoc />
        public override void Accept(IRecordStateVisitor visitor)
        {
            visitor.Handle(this);
        }
    }
}
