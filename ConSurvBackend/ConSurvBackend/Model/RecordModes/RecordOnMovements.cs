namespace ConSurvBackend.Core.Model.RecordModes
{
    public class RecordOnMovements : RecordMode
    {
        public double Threshold { get; internal set; }
        public override T Accept<T>(IRecordModeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

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
