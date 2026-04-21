using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using System;

namespace ConSurvBackend.Core.Misc
{
    /// <summary>
    /// Visitor that executes ONVIF commands (e.g. zoom, move) on a specific camera via the ONVIF protocol.
    /// </summary>
    internal class RunONVIFCommandVisitor : IONVIFCommandVisitor
    {
        private readonly Camera _Camera;

        /// <summary>
        /// Initializes a new instance of <see cref="RunONVIFCommandVisitor"/> for the given camera.
        /// </summary>
        /// <param name="camera">The camera on which ONVIF commands are executed.</param>
        public RunONVIFCommandVisitor(Camera camera)
        {
            this._Camera = camera;
        }

        /// <inheritdoc />
        public void Handle(Zoom zoom)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Handle(Move move)
        {
            throw new NotImplementedException();
        }
    }
}
