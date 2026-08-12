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

        /// <summary>Returns the value of the setting with the given key which belongs to the given user, or <see langword="null"/> if the user did not set it.</summary>
        /// <param name="userId">The id of the user the setting belongs to.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The stored value, or <see langword="null"/> if the user has no value for the key.</returns>
        public string? GetUserSetting(string userId, string key);

        /// <summary>Stores (inserts or updates) the value of the setting with the given key for the given user.</summary>
        /// <param name="userId">The id of the user the setting belongs to.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value to store.</param>
        public void SetUserSetting(string userId, string key, string value);

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
