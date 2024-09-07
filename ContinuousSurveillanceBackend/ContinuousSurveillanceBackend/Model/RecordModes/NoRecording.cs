namespace ContinuousSurveillanceBackend.Core.Model.RecordingModes
{
    public class NoRecording : RecordMode
    {

        public override T Accept<T>(IRecordModeVisitor<T> visitor) => visitor.Handle(this);

        public override void Accept(IRecordModeVisitor visitor) => visitor.Handle(this);

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => this.GetType().ToString();
    }
}
