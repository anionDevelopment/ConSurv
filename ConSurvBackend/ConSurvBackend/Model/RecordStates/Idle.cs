namespace ConSurvBackend.Core.Model.RecordStates
{
    /// <summary>
    /// Represents the state in which the camera stream is active but no footage is currently being recorded.
    /// </summary>
    public class Idle : RecordState
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
