namespace ConSurvBackend.Core.Model.RecordModes
{
    /// <summary>
    /// A <see cref="RecordMode"/> that starts recording only when motion is detected above a configurable
    /// sensitivity <see cref="Threshold"/>.
    /// </summary>
    public class RecordOnMovements : RecordMode
    {
        /// <summary>
        /// Gets the motion-detection sensitivity threshold above which recording is triggered.
        /// Higher values require more motion before recording begins.
        /// </summary>
        public double Threshold { get; internal set; }

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
