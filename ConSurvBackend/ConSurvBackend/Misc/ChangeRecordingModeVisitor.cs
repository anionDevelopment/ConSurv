using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;

namespace ConSurvBackend.Core.Misc
{
    /// <summary>
    /// Visitor that applies a recording-mode change for a specific camera by notifying the
    /// <see cref="IRuntimeData"/> so that <c>CameraManagementService</c> can react accordingly.
    /// </summary>
    internal class ChangeRecordingModeVisitor : IRecordModeVisitor
    {
        private readonly Camera _Camera;
        private readonly IRuntimeData _RuntimeData;

        /// <summary>
        /// Initializes a new instance of <see cref="ChangeRecordingModeVisitor"/>.
        /// </summary>
        /// <param name="camera">The camera whose recording mode is being changed.</param>
        /// <param name="runtimeData">Shared runtime state used to propagate the mode change.</param>
        public ChangeRecordingModeVisitor(Camera camera, IRuntimeData runtimeData)
        {
            this._Camera = camera;
            this._RuntimeData = runtimeData;
        }

        /// <inheritdoc />
        public void Handle(NoRecording noRecording)
        {
            //TODO use _RuntimeData to notify CameraManagementService about the changed state
        }

        /// <inheritdoc />
        public void Handle(RecordAlways recordAlways)
        {
            //TODO use _RuntimeData to notify CameraManagementService about the changed state
        }

        /// <inheritdoc />
        public void Handle(RecordOnMovements recordOnMovements)
        {
            //TODO use _RuntimeData to notify CameraManagementService about the changed state
        }
    }
}
