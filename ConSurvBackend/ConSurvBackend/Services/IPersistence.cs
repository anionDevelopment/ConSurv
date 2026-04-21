using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services;
using GRYLibrary.Core.APIServer.Services.Trans;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IPersistence : IExternalService, IAuthenticationServicePersistence<User>
    {
        /// <summary>
        /// Resets the persistence store to a clean state, removing all camera data.
        /// </summary>
        public void Reset();

        /// <summary>
        /// Persists a new camera in the store.
        /// </summary>
        /// <param name="camera">The camera to create.</param>
        public void CreateCamera(Camera camera);

        /// <summary>
        /// Updates an existing camera in the store.
        /// </summary>
        /// <param name="camera">The camera with updated values.</param>
        public void UpdateCamera(Camera camera);

        /// <summary>
        /// Removes the camera with the given id from the store.
        /// </summary>
        /// <param name="cameraId">The id of the camera to remove.</param>
        public void RemoveCamera(string cameraId);

        /// <summary>
        /// Returns all cameras stored in the persistence layer.
        /// </summary>
        /// <remarks>
        /// dictionary-key: camera-id
        /// dictionary-value: camera-object
        /// </remarks>
        /// <returns>A dictionary mapping camera id to the corresponding <see cref="Camera"/> object.</returns>
        public IDictionary<string, Camera> GetAllCameras();

        /// <summary>
        /// Determines whether an entity with the given id is a camera.
        /// </summary>
        /// <param name="id">The id to check.</param>
        /// <returns><c>true</c> if the id belongs to a camera; otherwise <c>false</c>.</returns>
        public bool IsCamera(string id);

    }
}
