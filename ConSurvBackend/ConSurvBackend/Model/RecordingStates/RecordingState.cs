using ConSurvBackend.Core.Model.DTOs;

namespace ConSurvBackend.Core.Model.RecordingStates
{
    public abstract class RecordingState
    {
        public abstract T Accept<T>(IRecordingStateVisitor<T> visitor);
        public abstract void Accept(IRecordingStateVisitor visitor);
        public  RecordingStateDTO ToDTO()
        {
            return new RecordingStateDTO()
            {
                RecordingState = this.GetType().Name,
            };
        }
    }
    public interface IRecordingStateVisitor<T>
    {
        T Handle(CurrentlyRecording currentlyRecording);
        T Handle(Idle idle);
        T Handle(Unavailable unavailable);
    }
    public interface IRecordingStateVisitor
    {
        void Handle(CurrentlyRecording currentlyRecording);
        void Handle(Idle idle);
        void Handle(Unavailable unavailable);
    }
}
