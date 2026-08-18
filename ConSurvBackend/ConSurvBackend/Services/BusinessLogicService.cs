using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.Internals;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.RecordStates;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Crypto;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConSurvBackend.Core.Services
{
    public class BusinessLogicService : IBusinessLogicService
    {
        private static readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
        private readonly IGRYLog _Log;
        private readonly IAuthenticationService<User> _AuthenticationService;
        private readonly IPersistence _Persistence;
        private readonly ITimeService _TimeService;
        private readonly IAuditLog _AuditLog;
        private readonly IRandomnessProvider _RandomnessProvider;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly IRuntimeData _RuntimeData;
        private readonly IApplicationConstants<Constants.CodeUnitSpecificConstants> _Constants;

        /// <inheritdoc />
        public string GetThemeOfUser(string userId)
        {
            // A value which is not stored (or which is not valid anymore because the set of the accepted values
            // changed) is treated as the default, so that the user-interface never has to deal with an unknown value.
            string? value = this._Persistence.GetUserSetting(userId, Constants.CodeUnitSpecificConstants.UserSettingKeyTheme);
            if (value == null || !Constants.CodeUnitSpecificConstants.Themes.Contains(value))
            {
                return Constants.CodeUnitSpecificConstants.ThemeSystem;
            }
            return value;
        }

        /// <inheritdoc />
        public void SetThemeOfUser(string userId, string theme)
        {
            if (!Constants.CodeUnitSpecificConstants.Themes.Contains(theme))
            {
                throw new BadRequestException($"'{theme}' is not a valid color-scheme. Valid are: {string.Join(", ", Constants.CodeUnitSpecificConstants.Themes)}.");
            }
            this._Persistence.SetUserSetting(userId, Constants.CodeUnitSpecificConstants.UserSettingKeyTheme, theme);
        }

        public BusinessLogicService(IPersistence persistence, IServerLog log, ITimeService timeService, IAuthenticationService<User> authenticationService, IRandomnessProvider randomnessProvider, IAuditLog auditLog, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, IRuntimeData runtimeData, IApplicationConstants<Constants.CodeUnitSpecificConstants> constants)
        {
            this._Persistence = persistence;
            this._Log = log.Logger;
            this._TimeService = timeService;
            this._AuthenticationService = authenticationService;
            this._RandomnessProvider = randomnessProvider;
            this._AuditLog = auditLog;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._RuntimeData = runtimeData;
            this._Constants = constants;
        }

        /// <summary>
        /// Characters that must not occur in a camera-name because they would break the FFmpeg command
        /// lines (shell/filter meta-characters) the camera-management-service builds from the name.
        /// </summary>
        private static readonly char[] _ForbiddenCameraNameCharacters = new char[] { '\'', '"', '|', '&', '*', '/', '\\' };

        /// <summary>
        /// Ensures the given camera-name does not contain any whitespace or meta-character that would
        /// break the FFmpeg command lines built from it. Throws a <see cref="BadRequestException"/>
        /// otherwise.
        /// </summary>
        /// <param name="name">The camera-name to validate.</param>
        /// <exception cref="BadRequestException">Thrown when the name is null or contains a forbidden character.</exception>
        private static void EnsureCameraNameIsValid(string name)
        {
            if (name is null)
            {
                throw new BadRequestException("The camera-name must not be null.");
            }
            foreach (char character in name)
            {
                if (char.IsWhiteSpace(character) || _ForbiddenCameraNameCharacters.Contains(character))
                {
                    throw new BadRequestException($"The camera-name \"{name}\" contains the forbidden character '{character}'. A camera-name must not contain whitespace or any of the following characters: single-quote, double-quote, pipe, ampersand, asterisk, slash, backslash.");
                }
            }
        }

        private static void Semaphore(SemaphoreSlim semaphore, Action action)
        {
            semaphore.Wait();
            try { action(); }
            finally { semaphore.Release(); }
        }

        private static T Semaphore<T>(SemaphoreSlim semaphore, Func<T> func)
        {
            semaphore.Wait();
            try { return func(); }
            finally { semaphore.Release(); }
        }

        /// <inheritdoc />
        public string CreateCamera(string name, string streamURL) => Semaphore(_Semaphore, () => this.CreateCameraCore(name, streamURL));
        private string CreateCameraCore(string name, string streamURL)
        {
            EnsureCameraNameIsValid(name);
            Camera camera = new Camera(this.GetUnusedId(streamURL), name);
            camera.VideoInformation.StreamURL = streamURL;
            this._Persistence.CreateCamera(camera);
            this._AuditLog.Logger.Log($"Created camera {camera.Id}.", LogLevel.Information);
            return camera.Id;
        }

        /// <summary>
        /// Computes a short deterministic id from an RTSP link by hashing the URL (after escaping embedded credentials) and taking the first 6 hex characters.
        /// </summary>
        /// <param name="rtspLink">The stream URL to derive the id from.</param>
        /// <returns>A 6-character hex string used as the camera id.</returns>
        private string GetId(string rtspLink)
        {
            return GRYLibrary.Core.Misc.Utilities.ByteArrayToHexString(new SHA256().Hash(GRYLibrary.Core.Misc.Utilities.StringToByteArray(Misc.Utilities.EscapeBasicAuthPasswords(rtspLink))))[..6];
        }

        /// <summary>
        /// Returns an id which is derived from the given stream-URL and which is not used by an already
        /// existing camera yet. Since the id is derived from the stream-URL alone, two cameras which are
        /// created with the same stream-URL (which is always the case for cameras created with the default
        /// stream-URL and adjusted afterwards) would otherwise get the same id and the second camera would
        /// replace the first one.
        /// </summary>
        /// <param name="rtspLink">The stream URL to derive the id from.</param>
        /// <returns>A 6-character hex string which is not used as a camera-id yet.</returns>
        /// <exception cref="InternalAlgorithmException">Thrown when no unused id could be found.</exception>
        private string GetUnusedId(string rtspLink)
        {
            IDictionary<string, Camera> existingCameras = this.GetAllCamerasCore();
            string result = this.GetId(rtspLink);
            uint discriminator = 0;
            while (existingCameras.ContainsKey(result))
            {
                discriminator = discriminator + 1;
                if (discriminator == uint.MaxValue)
                {
                    throw new InternalAlgorithmException($"Could not calculate an unused camera-id for the given stream-URL.");
                }
                result = this.GetId($"{rtspLink}#{discriminator}");
            }
            return result;
        }

        /// <inheritdoc />
        public (bool, Exception?) IsAvailable(Camera camera) => Semaphore(_Semaphore, () => this.IsAvailableCore(camera));
        private (bool, Exception?) IsAvailableCore(Camera camera)
        {
            if (this.GetCurrentRecordingInformationCore(camera) is Available)
            {
                return (true, null);
            }
            else
            {
                return (false, new DependencyNotAvailableException($"Camera {camera.Id} is not available."));
            }
        }

        /// <inheritdoc />
        public RecordState GetCurrentRecordingInformation(Camera camera) => Semaphore(_Semaphore, () => this.GetCurrentRecordingInformationCore(camera));
        private RecordState GetCurrentRecordingInformationCore(Camera camera)
        {
            try
            {
                return new Idle();//TODO
            }
            catch
            {
                return new Unavailable();
            }
        }

        /// <inheritdoc />
        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand) => Semaphore(_Semaphore, () => this.RunONVIFCommandCore(cameraId, onvifCommand));
        private void RunONVIFCommandCore(string cameraId, ONVIFCommand onvifCommand)
        {
            Camera camera = this.GetCameraByIdCore(cameraId);
            this._Log.Log($"Running ONVIF-command {onvifCommand.GetType().Name} on camera {cameraId}.", LogLevel.Debug);
            if (camera.VideoInformation.SupportsPTZViaONVIF)
            {
                (bool, Exception?) result = onvifCommand.Accept(new RunONVIFCommandVisitor(camera));
                if (!result.Item1)
                {
                    throw result.Item2!;
                }
            }
            else
            {
                throw new BadRequestException($"Camera '{camera.Name}' is not able to process ONVIF-commands.");
            }
        }

        /// <inheritdoc />
        public void RemoveCamera(string cameraId) => Semaphore(_Semaphore, () => this.RemoveCameraCore(cameraId));
        private void RemoveCameraCore(string cameraId)
        {
            //TODO check permission
            Camera camera = this.GetCameraByIdCore(cameraId);
            camera.RecordMode = new NoRecording();
            this._Persistence.RemoveCamera(cameraId);
            this._AuditLog.Logger.Log($"Removed camera {camera.Id}.", LogLevel.Information);
        }

        /// <inheritdoc />
        public void UpdateCamera(Camera camera) => Semaphore(_Semaphore, () => this.UpdateCameraCore(camera));
        private void UpdateCameraCore(Camera camera)
        {
            //TODO check permission
            EnsureCameraNameIsValid(camera.Name);
            this._Persistence.UpdateCamera(camera);
            camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RuntimeData));
            this._AuditLog.Logger.Log($"Updated camera {camera.Id}.", LogLevel.Information);//TODO add information about why and by whom this was done
        }

        /// <inheritdoc />
        public Camera GetCameraById(string cameraId) => Semaphore(_Semaphore, () => this.GetCameraByIdCore(cameraId));
        private Camera GetCameraByIdCore(string cameraId)
        {
            //TODO check permission
            if (this.GetAllCamerasCore().TryGetValue(cameraId, out Camera? value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"No camera available with id '{cameraId}'.");
            }
        }

        /// <inheritdoc />
        public double GetRateOfAvailableCameras() => Semaphore(_Semaphore, this.GetRateOfAvailableCamerasCore);
        private double GetRateOfAvailableCamerasCore()
        {
            IDictionary<string, Camera> cameras = this.GetAllCamerasCore();
            if (cameras.Count == 0)
            {
                return 0;
            }
            else
            {
                int amountOfAvailableCameras = cameras.Where(kvp => this.IsAvailableCore(kvp.Value).Item1).Count();
                return (double)amountOfAvailableCameras / cameras.Count;
            }
        }

        /// <inheritdoc />
        public string Register(string username, string password) => Semaphore(_Semaphore, () => this.RegisterCore(username, password));
        private string RegisterCore(string username, string password)
        {
            // Reject a name which is already taken. The database enforces this as well (the unique-constraint on
            // Users.Name), but only the check here can answer with a usable error instead of letting a
            // constraint-violation surface as an internal error. It also covers the transient persistence, which
            // has no constraint at all.
            // Attention: the Core-variant has to be used here, because this method already runs inside the
            // semaphore and a SemaphoreSlim is not re-entrant - calling the public method would deadlock.
            if (this.UserWithNameExistsCore(username))
            {
                throw new BadRequestException($"The username '{username}' is already taken.");
            }
            User newUser = User.CreateNewUser(username, this._AuthenticationService.Hash(password), this._TimeService);
            this._AuthenticationService.AddUserTyped(newUser);
            this._AuditLog.Logger.Log($"User \"{newUser.Name}\" (Id: {newUser.Id}) registered.", LogLevel.Information);
            return newUser.Id;
        }

        /// <inheritdoc />
        public bool UserWithNameExists(string username) => Semaphore(_Semaphore, () => this.UserWithNameExistsCore(username));
        private bool UserWithNameExistsCore(string username)
        {
            return this._Persistence.UserWithNameExists(username);
        }

        /// <inheritdoc />
        public IDictionary<string, Camera> GetAllCameras() => Semaphore(_Semaphore, this.GetAllCamerasCore);
        private IDictionary<string, Camera> GetAllCamerasCore()
        {
            //TODO check permission
            return this._Persistence.GetAllCameras();
        }

        /// <inheritdoc />
        public CameraDTO ToDTO(Camera camera) => Semaphore(_Semaphore, () => this.ToDTOCore(camera));
        private CameraDTO ToDTOCore(Camera camera)
        {
            return new CameraDTO()
            {
                CameraId = camera.Id,
                Name = camera.Name,
                RecordModeDTO = camera.RecordMode.ToDTO(),
                VideoInformationDTO = camera.VideoInformation.ToDTO(),
                RecordStateDTO = this.GetCurrentRecordingInformationCore(camera).ToDTO(),
            };
        }

        /// <inheritdoc />
        public void EnsureUserHasRole(string userId, string roleId) => Semaphore(_Semaphore, () => this.EnsureUserHasRoleCore(userId, roleId));
        private void EnsureUserHasRoleCore(string userId, string roleId)
        {
            this._AuditLog.Logger.Log($"Add role with id {roleId} to user with id {userId}...", LogLevel.Information);
            this._AuthenticationService.EnsureUserHasRole(userId, roleId);
            //TODO add information about why and by whom this was done
        }

        /// <inheritdoc />
        public void EnsureUserDoesNotHaveRole(string userId, string roleId) => Semaphore(_Semaphore, () => this.EnsureUserDoesNotHaveRoleCore(userId, roleId));
        private void EnsureUserDoesNotHaveRoleCore(string userId, string roleId)
        {
            this._AuditLog.Logger.Log($"Unassign role with id {roleId} from user with id {userId}.", LogLevel.Information);
            this._AuthenticationService.EnsureUserDoesNotHaveRole(userId, roleId);
            //TODO add information about why and by whom this was done to auditlog
        }

        /// <inheritdoc />
        public User GetUser(string userId) => Semaphore(_Semaphore, () => this.GetUserCore(userId));
        private User GetUserCore(string userId)
        {
            return this._AuthenticationService.GetUserTyped(userId);
        }

        /// <inheritdoc />
        public AccessToken Login(string username, string password) => Semaphore(_Semaphore, () => this.LoginCore(username, password));
        private AccessToken LoginCore(string username, string password)
        {
            return this._AuthenticationService.Login(username, password);
        }

        /// <inheritdoc />
        public IDictionary<string, IList<string>> GetVideos() => Semaphore(_Semaphore, this.GetVideosCore);
        private IDictionary<string, IList<string>> GetVideosCore()
        {
            Dictionary<string, IList<string>> result = new Dictionary<string, IList<string>>();
            string cameraDataFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData");
            if (!Directory.Exists(cameraDataFolder))
            {
                return result;
            }
            foreach (string folder in Directory.GetDirectories(cameraDataFolder))
            {
                string cameraId = new DirectoryInfo(folder).Name;
                List<string> list = new List<string>();
                // The recordings-folder only exists for cameras which are (or were) recording, so its
                // absence is a regular state and must not let the entire listing fail.
                string recordingsFolder = Path.Combine(folder, "Recordings");
                if (Directory.Exists(recordingsFolder))
                {
                    foreach (string file in Directory.GetFiles(recordingsFolder))
                    {
                        list.Add(Path.GetFileName(file));
                    }
                }
                result.Add(cameraId, list);
            }
            return result;
        }

        /// <summary>
        /// Returns the absolute path of a recording of a camera. The camera-id is resolved against the
        /// existing cameras and the file-name must be a plain file-name, so that neither of both can be
        /// used to leave the recordings-folder of the camera.
        /// </summary>
        /// <param name="cameraId">The id of the camera the recording belongs to.</param>
        /// <param name="filename">The plain file-name of the recording.</param>
        /// <returns>The absolute path of the recording.</returns>
        /// <exception cref="BadRequestException">Thrown when the file-name is not a plain file-name.</exception>
        private string GetRecordingFile(string cameraId, string filename)
        {
            Camera camera = this.GetCameraByIdCore(cameraId);
            if (string.IsNullOrWhiteSpace(filename) || filename.Contains('/') || filename.Contains('\\') || filename != Path.GetFileName(filename) || filename == "." || filename == "..")
            {
                throw new BadRequestException($"\"{filename}\" is not a valid name of a recording.");
            }
            return Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Recordings", filename);
        }

        /// <inheritdoc />
        public void RemoveVideo(string cameraId, string filename) => Semaphore(_Semaphore, () => this.RemoveVideoCore(cameraId, filename));
        private void RemoveVideoCore(string cameraId, string filename)
        {
            string fullPath = this.GetRecordingFile(cameraId, filename);
            if (File.Exists(fullPath))
            {
                this._Log.Log($"Removing recording \"{filename}\" of camera {cameraId}.", LogLevel.Debug);
                File.Delete(fullPath);
            }
            else
            {
                throw new BadRequestException($"File \"{filename}\" does not exist for camera \"{cameraId}\".");
            }
        }

        /// <inheritdoc />
        public byte[] GetPreviewOfVideo(string cameraId, string filename) => Semaphore(_Semaphore, () => this.GetPreviewOfVideoCore(cameraId, filename));
        private byte[] GetPreviewOfVideoCore(string cameraId, string filename)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public byte[] GetVideo(string cameraId, string filename) => Semaphore(_Semaphore, () => this.GetVideoCore(cameraId, filename));
        private byte[] GetVideoCore(string cameraId, string filename)
        {
            string fullPath = this.GetRecordingFile(cameraId, filename);
            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }
            else
            {
                throw new BadRequestException($"File \"{filename}\" does not exist for camera \"{cameraId}\".");
            }
        }
    }
}
