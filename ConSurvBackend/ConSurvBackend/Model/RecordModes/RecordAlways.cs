namespace ConSurvBackend.Core.Model.RecordModes
{
    /// <summary>
    /// A <see cref="RecordMode"/> that instructs the camera to record continuously,
    /// regardless of any detected activity.
    /// </summary>
    public class RecordAlways : RecordMode
    {
        /// <inheritdoc />
        public override T Accept<T>(IRecordModeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        /// <inheritdoc />
        public override void Accept(IRecordModeVisitor visitor)
        {
            visitor.Handle(this);
        }
    }
}
