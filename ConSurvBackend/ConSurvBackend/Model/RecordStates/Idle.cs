namespace ConSurvBackend.Core.Model.RecordStates
{
    public class Idle : RecordState
    {
        public override T Accept<T>(IRecordStateVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public override void Accept(IRecordStateVisitor visitor)
        {
            visitor.Handle(this);
        }
    }
}
