namespace ConSurvBackend.Core.Model.RecordModes
{
    /// <summary>
    /// A <see cref="RecordMode"/> that disables all recording for the camera.
    /// The camera stream remains active but no footage is written to disk.
    /// </summary>
    public class NoRecording : RecordMode
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

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().ToString();
        }
    }
}
