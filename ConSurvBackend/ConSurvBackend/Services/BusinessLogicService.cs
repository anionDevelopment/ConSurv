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
            Camera camera = new Camera(this.GetId(streamURL), name);
            camera.VideoInformation.StreamURL = streamURL;
            this.GetAllCamerasCore()[camera.Id] = camera;
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
            if (this.GetAllCamerasCore().Count == 0)
            {
                return 0;
            }
            else
            {
                return this.GetAllCamerasCore().Where(kvp => this.IsAvailableCore(kvp.Value).Item1).Count() / this.GetAllCamerasCore().Count;
            }
        }

        /// <inheritdoc />
        public string Register(string username, string password) => Semaphore(_Semaphore, () => this.RegisterCore(username, password));
        private string RegisterCore(string username, string password)
        {
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
            foreach (string folder in Directory.GetDirectories(Path.Combine(this._Constants.GetDataFolder(), "CameraData")))
            {
                string cameraId = new DirectoryInfo(folder).Name;
                List<string> list = new List<string>();
                string recordingsFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", cameraId, "Recordings");
                foreach (string file in Directory.GetFiles(recordingsFolder))
                {
                    list.Add(file);
                }
                result.Add(cameraId, list);
            }
            return result;
        }

        /// <inheritdoc />
        public void RemoveVideo(string cameraId, string filename) => Semaphore(_Semaphore, () => this.RemoveVideoCore(cameraId, filename));
        private void RemoveVideoCore(string cameraId, string filename)
        {
            string fullPath = Path.Combine(this._Constants.GetDataFolder(), "CameraData", cameraId, "Recordings", filename);
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
            string fullPath = Path.Combine(this._Constants.GetDataFolder(), "CameraData", cameraId, "Recordings", filename);
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
