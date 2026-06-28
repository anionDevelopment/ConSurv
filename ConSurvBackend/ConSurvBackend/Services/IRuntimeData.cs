using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.Internals;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IRuntimeData
    {
        /// <summary>
        /// Returns a snapshot of the internal runtime state for all cameras.
        /// </summary>
        /// <returns>A dictionary mapping camera id to its <see cref="CameraInternalsBase"/>.</returns>
        public IDictionary<string, CameraInternalsBase> GetCameraInternals();

        /// <summary>
        /// Returns the internal runtime state for the specified camera.
        /// </summary>
        /// <param name="cameraId">The id of the camera.</param>
        /// <returns>The <see cref="CameraInternalsBase"/> for the specified camera.</returns>
        public CameraInternalsBase GetCameraInternals(string cameraId);

        /// <summary>
        /// Stores or replaces the internal runtime state for the camera referenced by <paramref name="cameraInternals"/>.
        /// </summary>
        /// <param name="cameraInternals">The camera internals object to store; its <c>Camera.Id</c> is used as the key.</param>
        public void SetCameraInternals(CameraInternalsBase cameraInternals);

        /// <summary>
        /// Checks whether internal runtime data is available for the given camera.
        /// </summary>
        /// <param name="cameraId">The id of the camera to check.</param>
        /// <returns><c>true</c> if internals exist for the camera; otherwise <c>false</c>.</returns>
        public bool InternalsAreAvailable(string cameraId);

        /// <summary>
        /// Returns the fallback preview image shown when no real preview is available for a camera.
        /// </summary>
        /// <returns>A byte array containing the fallback JPEG image.</returns>
        public byte[] GetPreviewFallbackPicture();

        /// <summary>
        /// Adds a new preview frame to the preview queue of the specified camera.
        /// </summary>
        /// <param name="cameraId">The id of the camera.</param>
        /// <param name="preview">The preview to enqueue.</param>
        public void AddPreview(string cameraId, Preview preview);

        /// <summary>
        /// Retrieves the most recently added preview frame for the specified camera.
        /// Falls back to the fallback picture if the queue is empty.
        /// </summary>
        /// <param name="cameraId">The id of the camera.</param>
        /// <returns>The latest <see cref="Preview"/> available for the camera.</returns>
        public Preview GetLatestPreview(string cameraId);
    }
}
