using ConSurvBackend.Core.Model.DTOs;
using System;

namespace ConSurvBackend.Core.Model.RecordModes
{
    public abstract class RecordMode : IEquatable<RecordMode>
    {
        public abstract T Accept<T>(IRecordModeVisitor<T> visitor);
        public abstract void Accept(IRecordModeVisitor visitor);

        public override bool Equals(object obj)
        {
            return this.Equals(obj as RecordMode);
        }

        public bool Equals(RecordMode other)
        {
            if (other == null)
            {
                return false;
            };
            if (!this.GetType().Equals(other.GetType()))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public RecordModeDTO ToDTO()
        {

            return new RecordModeDTO()
            {
                RecordMode = this.GetType().Name,
            };
        }
    }
    public interface IRecordModeVisitor<T>
    {
        T Handle(NoRecording noRecording);
        T Handle(RecordAlways recordAlways);
    }
    public interface IRecordModeVisitor
    {
        void Handle(NoRecording noRecording);
        void Handle(RecordAlways recordAlways);
    }
}
