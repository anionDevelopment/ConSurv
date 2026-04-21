using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.Internals
{
    /// <summary>
    /// Represents the state in which a camera's backing processes are not running
    /// and the stream is currently unavailable.
    /// </summary>
    public class NotAvailable : CameraInternalsBase
    {
        /// <summary>
        /// Initializes the <see cref="NotAvailable"/> state for the given camera.
        /// </summary>
        /// <param name="camera">The camera whose processes are not available.</param>
        public NotAvailable(Camera camera) : base(camera)
        {
        }

        /// <inheritdoc />
        public override void Accept(ICameraInternalsBaseVisitor visitor)
        {
            visitor.Handle(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(ICameraInternalsBaseVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
