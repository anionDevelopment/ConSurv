using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.Internals
{
    /// <summary>
    /// Abstract base that tracks the runtime availability state of a camera's backing processes.
    /// Concrete subclasses use the Visitor pattern to let callers branch on <see cref="Available"/>
    /// vs. <see cref="NotAvailable"/> without casting.
    /// </summary>
    public abstract class CameraInternalsBase
    {
        public  Camera Camera { get;private set; }

        /// <summary>
        /// Initializes the base with the camera this state belongs to.
        /// </summary>
        /// <param name="camera">The camera whose runtime state is represented.</param>
        public CameraInternalsBase(Camera camera)
        {
            this.Camera = camera;
        }

        /// <summary>
        /// Accepts a void visitor and dispatches to the matching <c>Handle</c> overload.
        /// </summary>
        /// <param name="visitor">The visitor to dispatch to.</param>
        public abstract void Accept(ICameraInternalsBaseVisitor visitor);

        /// <summary>
        /// Accepts a returning visitor and dispatches to the matching <c>Handle</c> overload.
        /// </summary>
        /// <typeparam name="T">The return type produced by the visitor.</typeparam>
        /// <param name="visitor">The visitor to dispatch to.</param>
        /// <returns>The value returned by the visitor's <c>Handle</c> method.</returns>
        public abstract T Accept<T>(ICameraInternalsBaseVisitor<T> visitor);
    }

    /// <summary>
    /// Visitor interface for performing side-effecting operations on <see cref="CameraInternalsBase"/> subclasses.
    /// </summary>
    public interface ICameraInternalsBaseVisitor
    {
        void Handle(Available available);
        void Handle(NotAvailable notAvailable);
    }

    /// <summary>
    /// Visitor interface for querying <see cref="CameraInternalsBase"/> subclasses and producing a result.
    /// </summary>
    /// <typeparam name="T">The type of value returned by each handler.</typeparam>
    public interface ICameraInternalsBaseVisitor<T>
    {
        T Handle(Available available);
        T Handle(NotAvailable notAvailable);
    }
}
