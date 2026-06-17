using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordStates;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IBusinessLogicService
    {
        /// <summary>
        /// Creates a new camera with the given name and stream URL.
        /// </summary>
        /// <param name="name">The display name of the camera.</param>
        /// <param name="streamURL">The RTSP or HTTP stream URL of the camera.</param>
        /// <returns>Returns the id of the created camera</returns>
        public string CreateCamera(string name, string streamURL);

        /// <summary>
        /// Checks whether the given camera is currently reachable.
        /// </summary>
        /// <param name="camera">The camera to check.</param>
        /// <returns>A tuple where the first element indicates availability and the second element contains the exception if unavailable, or <c>null</c> if available.</returns>
        public (bool,Exception?) IsAvailable(Camera camera);

        /// <summary>
        /// Retrieves the current recording state of the given camera.
        /// </summary>
        /// <param name="camera">The camera to query.</param>
        /// <returns>The current <see cref="RecordState"/> of the camera.</returns>
        public RecordState GetCurrentRecordingInformation(Camera camera);

        /// <summary>
        /// Persists changes made to an existing camera and applies the updated recording mode.
        /// </summary>
        /// <param name="camera">The camera object with updated values.</param>
        public void UpdateCamera(Camera camera);

        /// <summary>
        /// Removes the camera with the specified id from the system.
        /// </summary>
        /// <param name="cameraId">The id of the camera to remove.</param>
        public void RemoveCamera(string cameraId);

        /// <summary>
        /// Sends an ONVIF command to the specified camera.
        /// </summary>
        /// <param name="cameraId">The id of the target camera.</param>
        /// <param name="onvifCommand">The ONVIF command to execute.</param>
        /// <exception cref="GRYLibrary.Core.Exceptions.BadRequestException">Thrown when the camera does not support ONVIF commands.</exception>
        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand);

        /// <summary>
        /// Retrieves a camera by its id.
        /// </summary>
        /// <param name="cameraId">The id of the camera to retrieve.</param>
        /// <returns>The <see cref="Camera"/> with the specified id.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when no camera with the given id exists.</exception>
        public Camera GetCameraById(string cameraId);

        /// <summary>
        /// Returns the fraction of all registered cameras that are currently available.
        /// </summary>
        /// <returns>A value between 0.0 and 1.0, where 1.0 means all cameras are available. Returns 0 if no cameras are registered.</returns>
        public double GetRateOfAvailableCameras();

        /// <summary>
        /// Registers a new user with the given credentials.
        /// </summary>
        /// <param name="username">The desired username.</param>
        /// <param name="password">The plain-text password, which will be hashed before storage.</param>
        /// <returns>The id of the newly created user.</returns>
        public string Register(string username, string password);

        /// <summary>
        /// Checks whether a user with the given username already exists.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if a user with that name exists; otherwise <c>false</c>.</returns>
        public bool UserWithNameExists(string username);

        /// <summary>
        /// Returns all cameras registered in the system.
        /// </summary>
        /// <returns>A dictionary mapping camera id to the corresponding <see cref="Camera"/> object.</returns>
        public IDictionary<string, Camera> GetAllCameras();

        /// <summary>
        /// Converts a <see cref="Camera"/> domain object to its DTO representation.
        /// </summary>
        /// <param name="camera">The camera to convert.</param>
        /// <returns>A <see cref="CameraDTO"/> populated with data from the camera.</returns>
        public CameraDTO ToDTO(Camera camera);

        /// <summary>
        /// Ensures that the specified user has the specified role assigned, adding the role if necessary.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="roleId">The id of the role.</param>
        public void EnsureUserHasRole(string userId, string roleId);

        /// <summary>
        /// Ensures that the specified user does not have the specified role.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="roleId">The id of the role.</param>
        public void EnsureUserDoesNotHaveRole(string userId, string roleId);

        /// <summary>
        /// Authenticates a user and returns a new access token on success.
        /// </summary>
        /// <param name="name">The username.</param>
        /// <param name="v">The plain-text password.</param>
        /// <returns>A valid <see cref="AccessToken"/> for the authenticated user.</returns>
        /// <exception cref="GRYLibrary.Core.Exceptions.InvalidCredentialsException">Thrown when the credentials are invalid.</exception>
        public AccessToken Login(string name, string v);

        /// <summary>
        /// Retrieves a user by their id.
        /// </summary>
        /// <param name="userId">The id of the user to retrieve.</param>
        /// <returns>The <see cref="User"/> with the specified id.</returns>
        public User GetUser(string userId);

        /// <summary>
        /// Returns all recorded videos grouped by camera.
        /// </summary>
        /// <remarks>
        /// key: camera-id
        /// value: filenames of videos
        /// </remarks>
        public IDictionary<string, IList<string>> GetVideos();

        /// <summary>
        /// Deletes a specific recorded video for a camera.
        /// </summary>
        /// <param name="cameraId">The id of the camera that owns the video.</param>
        /// <param name="filename">The filename of the video to delete.</param>
        /// <exception cref="GRYLibrary.Core.Exceptions.BadRequestException">Thrown when the specified file does not exist.</exception>
        public void RemoveVideo(string cameraId,string filename);

        /// <summary>
        /// Returns a preview image of the specified recorded video.
        /// </summary>
        /// <param name="cameraId">The id of the camera that owns the video.</param>
        /// <param name="filename">The filename of the video.</param>
        /// <returns>A byte array containing the preview image data.</returns>
        byte[] GetPreviewOfVideo(string cameraId, string filename);

        /// <summary>
        /// Returns the raw bytes of the specified recorded video file.
        /// </summary>
        /// <param name="cameraId">The id of the camera that owns the video.</param>
        /// <param name="filename">The filename of the video to retrieve.</param>
        /// <returns>A byte array containing the video file content.</returns>
        /// <exception cref="GRYLibrary.Core.Exceptions.BadRequestException">Thrown when the specified file does not exist.</exception>
        byte[] GetVideo(string cameraId, string filename);
    }
}
