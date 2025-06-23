using ConSurvBackend.Core.Model.DTOs;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Model.RecordModes
{
    public abstract class RecordMode : IEquatable<RecordMode>
    {
        public RecordMode() { }
        public abstract T Accept<T>(IRecordModeVisitor<T> visitor);
        public abstract void Accept(IRecordModeVisitor visitor);

        private static IDictionary<byte, Type> _IntegerMapping = new Dictionary<byte, Type>()
        {
            { 0, typeof(NoRecording) },
            { 1, typeof(RecordAlways) },
            { 2, typeof(RecordOnMovements) },
        };
        public static byte ToNumber(Type type)
        {
           foreach (KeyValuePair<byte, Type> kvp in _IntegerMapping)
            {
                if (kvp.Value.Equals(type))
                {
                    return kvp.Key;
                }
            }
            throw new KeyNotFoundException($"No number found for type {type.Name}");
        }
        public static Type FromNumber(byte number)
        {
            return _IntegerMapping[number];
        }
        public static RecordMode FromNumberToInstance(byte number)
        {
            return (RecordMode) Activator.CreateInstance( FromNumber(number));
        }
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
        T Handle(RecordOnMovements recordOnMovements);
    }
    public interface IRecordModeVisitor
    {
        void Handle(NoRecording noRecording);
        void Handle(RecordAlways recordAlways);
        void Handle(RecordOnMovements recordOnMovements);
    }
}
