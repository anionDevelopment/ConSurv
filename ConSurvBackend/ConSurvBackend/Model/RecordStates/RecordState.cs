using ConSurvBackend.Core.Model.DTOs;

namespace ConSurvBackend.Core.Model.RecordStates
{
    public abstract class RecordState
    {
        public abstract T Accept<T>(IRecordStateVisitor<T> visitor);
        public abstract void Accept(IRecordStateVisitor visitor);
        public  RecordStateDTO ToDTO()
        {
            return new RecordStateDTO()
            {
                RecordState = this.GetType().Name,
            };
        }
    }
    public interface IRecordStateVisitor<T>
    {
        T Handle(CurrentlyRecording currentlyRecording);
        T Handle(Idle idle);
        T Handle(Unavailable unavailable);
    }
    public interface IRecordStateVisitor
    {
        void Handle(CurrentlyRecording currentlyRecording);
        void Handle(Idle idle);
        void Handle(Unavailable unavailable);
    }
}
