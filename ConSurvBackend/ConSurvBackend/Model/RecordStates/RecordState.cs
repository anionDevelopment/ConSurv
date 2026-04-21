using ConSurvBackend.Core.Model.DTOs;

namespace ConSurvBackend.Core.Model.RecordStates
{
    /// <summary>
    /// Abstract base class representing the current recording state of a camera at runtime.
    /// Concrete subclasses use the Visitor pattern to allow callers to branch on state without casting.
    /// </summary>
    public abstract class RecordState
    {
        /// <summary>
        /// Accepts a returning visitor and dispatches to the concrete subclass handler.
        /// </summary>
        /// <typeparam name="T">The return type produced by the visitor.</typeparam>
        /// <param name="visitor">The visitor to dispatch to.</param>
        /// <returns>The value returned by the visitor's <c>Handle</c> method.</returns>
        public abstract T Accept<T>(IRecordStateVisitor<T> visitor);

        /// <summary>
        /// Accepts a void visitor and dispatches to the concrete subclass handler.
        /// </summary>
        /// <param name="visitor">The visitor to dispatch to.</param>
        public abstract void Accept(IRecordStateVisitor visitor);

        /// <summary>
        /// Converts this record state to a <see cref="RecordStateDTO"/> that encodes
        /// the concrete type name for transport over the API.
        /// </summary>
        /// <returns>A <see cref="RecordStateDTO"/> representing this record state.</returns>
        public  RecordStateDTO ToDTO()
        {
            return new RecordStateDTO()
            {
                RecordState = this.GetType().Name,
            };
        }
    }
    /// <summary>
    /// Visitor interface for querying a <see cref="RecordState"/> and producing a result.
    /// </summary>
    /// <typeparam name="T">The type of value returned by each handler.</typeparam>
    public interface IRecordStateVisitor<T>
    {
        T Handle(CurrentlyRecording currentlyRecording);
        T Handle(Idle idle);
        T Handle(Unavailable unavailable);
    }

    /// <summary>
    /// Visitor interface for performing side-effecting operations on a <see cref="RecordState"/>.
    /// </summary>
    public interface IRecordStateVisitor
    {
        void Handle(CurrentlyRecording currentlyRecording);
        void Handle(Idle idle);
        void Handle(Unavailable unavailable);
    }
}
