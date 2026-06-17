namespace ConSurvBackend.Core.Model.RecordStates
{
    /// <summary>
    /// Represents the state in which the camera's backing processes are not running
    /// and recording is therefore not possible.
    /// </summary>
    public class Unavailable : RecordState
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
