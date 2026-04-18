using ConSurvBackend.Core.Model.DTOs;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Model.RecordModes
{
    /// <summary>
    /// Abstract base class for all recording modes. Concrete subclasses determine
    /// when and how a camera records footage. Equality is based solely on the
    /// runtime type — two instances of the same subclass are always considered equal.
    /// </summary>
    public abstract class RecordMode : IEquatable<RecordMode>
    {
        public RecordMode() { }

        /// <summary>
        /// Accepts a returning visitor and dispatches to the concrete subclass handler.
        /// </summary>
        /// <typeparam name="T">The return type produced by the visitor.</typeparam>
        /// <param name="visitor">The visitor to dispatch to.</param>
        /// <returns>The value returned by the visitor's <c>Handle</c> method.</returns>
        public abstract T Accept<T>(IRecordModeVisitor<T> visitor);

        /// <summary>
        /// Accepts a void visitor and dispatches to the concrete subclass handler.
        /// </summary>
        /// <param name="visitor">The visitor to dispatch to.</param>
        public abstract void Accept(IRecordModeVisitor visitor);

        private static readonly IDictionary<byte, Type> _IntegerMapping = new Dictionary<byte, Type>()
        {
            { 0, typeof(NoRecording) },
            { 1, typeof(RecordAlways) },
            { 2, typeof(RecordOnMovements) },
        };
        /// <summary>
        /// Returns the numeric identifier that corresponds to the given <see cref="RecordMode"/> subtype.
        /// </summary>
        /// <param name="type">The concrete <see cref="RecordMode"/> subtype to look up.</param>
        /// <returns>The numeric identifier for <paramref name="type"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="type"/> is not registered in the mapping.</exception>
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
        /// <summary>
        /// Returns the <see cref="RecordMode"/> subtype that corresponds to the given numeric identifier.
        /// </summary>
        /// <param name="number">The numeric identifier to look up.</param>
        /// <returns>The <see cref="Type"/> of the matching <see cref="RecordMode"/> subclass.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="number"/> is not a registered identifier.</exception>
        public static Type FromNumber(byte number)
        {
            return _IntegerMapping[number];
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="RecordMode"/> subclass
        /// that corresponds to the given numeric identifier.
        /// </summary>
        /// <param name="number">The numeric identifier of the desired record mode.</param>
        /// <returns>A new instance of the matching <see cref="RecordMode"/> subclass.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="number"/> is not a registered identifier.</exception>
        public static RecordMode FromNumberToInstance(byte number)
        {
            return (RecordMode) Activator.CreateInstance( FromNumber(number));
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as RecordMode);
        }

        /// <summary>
        /// Determines whether this <see cref="RecordMode"/> is equal to another by comparing their runtime types.
        /// </summary>
        /// <param name="other">The other <see cref="RecordMode"/> to compare with.</param>
        /// <returns><see langword="true"/> if both instances have the same concrete type; otherwise <see langword="false"/>.</returns>
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

        /// <summary>
        /// Converts this record mode to a <see cref="RecordModeDTO"/> that encodes
        /// the concrete type name for transport over the API.
        /// </summary>
        /// <returns>A <see cref="RecordModeDTO"/> representing this record mode.</returns>
        public RecordModeDTO ToDTO()
        {

            return new RecordModeDTO()
            {
                RecordMode = this.GetType().Name,
            };
        }
    }
    /// <summary>
    /// Visitor interface for querying a <see cref="RecordMode"/> and producing a result.
    /// </summary>
    /// <typeparam name="T">The type of value returned by each handler.</typeparam>
    public interface IRecordModeVisitor<T>
    {
        T Handle(NoRecording noRecording);
        T Handle(RecordAlways recordAlways);
        T Handle(RecordOnMovements recordOnMovements);
    }

    /// <summary>
    /// Visitor interface for performing side-effecting operations on a <see cref="RecordMode"/>.
    /// </summary>
    public interface IRecordModeVisitor
    {
        void Handle(NoRecording noRecording);
        void Handle(RecordAlways recordAlways);
        void Handle(RecordOnMovements recordOnMovements);
    }
}
