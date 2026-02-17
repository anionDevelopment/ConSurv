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

namespace ConSurvBackend.Core.Services
{
    public class BusinessLogicService : IBusinessLogicService
    {
        private static readonly object _LockObject = new object();
        private readonly IGRYLog _Log;
        private readonly IAuthenticationService<User> _AuthenticationService;
        private readonly IPersistence _Persistence;
        private readonly ITimeService _TimeService;
        private readonly IAuditLog _AuditLog;
        private readonly IRandomnessProvider _RandomnessProvider;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly IRuntimeData _RuntimeData;
        private readonly IApplicationConstants<Constants.CodeUnitSpecificConstants> _Constants;

        public BusinessLogicService(IPersistence persistence, IGRYLog log, ITimeService timeService, IAuthenticationService<User> authenticationService, IRandomnessProvider randomnessProvider, IAuditLog auditLog, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, IRuntimeData runtimeData, IApplicationConstants<Constants.CodeUnitSpecificConstants> constants)
        {
            this._Persistence = persistence;
            this._Log = log;
            this._TimeService = timeService;
            this._AuthenticationService = authenticationService;
            this._RandomnessProvider = randomnessProvider;
            this._AuditLog = auditLog;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._RuntimeData = runtimeData;
            this._Constants = constants;
        }
        public string CreateCamera(string name, string streamURL)
        {
            Camera camera = new Camera(this.GetId(streamURL), name);
            camera.VideoInformation.StreamURL = streamURL;
            this.GetAllCameras()[camera.Id] = camera;
            this._Persistence.CreateCamera(camera);
            this._AuditLog.AuditLogger.Log($"Created camera {camera.Id}.", LogLevel.Information);
            return camera.Id;
        }

        private string GetId(string rtspLink)
        {
            return GRYLibrary.Core.Misc.Utilities.ByteArrayToHexString(new SHA256().Hash(GRYLibrary.Core.Misc.Utilities.StringToByteArray(Misc.Utilities.EscapeBasicAuthPasswords(rtspLink))))[..6];
        }

        public (bool, Exception?) IsAvailable(Camera camera)
        {
            if (this.GetCurrentRecordingInformation(camera) is Available)
            {
                return (true, null);
            }
            else
            {
                return (false, new DependencyNotAvailableException($"Camera {camera.Id} is not available."));
            }
        }

        public RecordState GetCurrentRecordingInformation(Camera camera)
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
        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand)
        {
            Camera camera = this.GetCameraById(cameraId);
            if (camera.VideoInformation.IsONVIFCamera)
            {
                onvifCommand.Accept(new RunONVIFCommandVisitor(camera));
            }
            else
            {
                throw new BadRequestException($"Camera '{camera.Name}' is not able to process ONVIF-commands.");
            }
        }

        public void RemoveCamera(string cameraId)
        {
            //TODO check permission
            Camera camera = this.GetCameraById(cameraId);
            camera.RecordMode = new NoRecording();
            this._Persistence.RemoveCamera(cameraId);
            this._AuditLog.AuditLogger.Log($"Removed camera {camera.Id}.", LogLevel.Information);
        }

        public void UpdateCamera(Camera camera)
        {
            //TODO check permission
            this._Persistence.UpdateCamera(camera);
            camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RuntimeData));
            this._AuditLog.AuditLogger.Log($"Updated camera {camera.Id}.", LogLevel.Information);//TODO add information about why and by whom this was done
        }

        public Camera GetCameraById(string cameraId)
        {
            //TODO check permission
            if (this.GetAllCameras().TryGetValue(cameraId, out Camera? value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"No camera available with id '{cameraId}'.");
            }
        }

        public double GetRateOfAvailableCameras()
        {
            if (this.GetAllCameras().Count == 0)
            {
                return 0;
            }
            else
            {
                return this.GetAllCameras().Where(kvp => this.IsAvailable(kvp.Value).Item1).Count() / this.GetAllCameras().Count;
            }
        }

        public string Register(string username, string password)
        {
            lock (_LockObject)
            {
                User newUser = User.CreateNewUser(username, this._AuthenticationService.Hash(password), this._TimeService);
                this._AuthenticationService.AddUserTyped(newUser);
                this._AuditLog.AuditLogger.Log($"User \"{newUser.Name}\" (Id: {newUser.Id}) registered.", LogLevel.Information);
                return newUser.Id;
            }
        }

        public bool UserWithNameExists(string username)
        {
            return this._Persistence.UserWithNameExists(username);
        }

        public IDictionary<string, Camera> GetAllCameras()
        {
            //TODO check permission
            return this._Persistence.GetAllCameras();
        }
        public CameraDTO ToDTO(Camera camera)
        {
            return new CameraDTO()
            {
                CameraId = camera.Id,
                Name = camera.Name,
                RecordModeDTO = camera.RecordMode.ToDTO(),
                VideoInformationDTO = camera.VideoInformation.ToDTO(),
                RecordStateDTO = this.GetCurrentRecordingInformation(camera).ToDTO(),
            };
        }

        public void EnsureUserHasRole(string userId, string roleId)
        {
            this._AuditLog.AuditLogger.Log($"Add role with id {roleId} to user with id {userId}...", LogLevel.Information);
            this._AuthenticationService.EnsureUserHasRole(userId, roleId);
            //TODO add information about why and by whom this was done
        }

        public void EnsureUserDoesNotHaveRole(string userId, string roleId)
        {
            this._AuditLog.AuditLogger.Log($"Unassign role with id {roleId} from user with id {userId}.", LogLevel.Information);
            this._AuthenticationService.EnsureUserDoesNotHaveRole(userId, roleId);
            //TODO add information about why and by whom this was done to auditlog
        }
        public User GetUser(string userId)
        {
            return this._AuthenticationService.GetUserTyped(userId);
        }

        public AccessToken Login(string username, string password)
        {
            return this._AuthenticationService.Login(username, password);
        }

        public IDictionary<string, IList<string>> GetVideos()
        {
            Dictionary<string, IList<string>> result = new Dictionary<string, IList<string>>();
            foreach (string folder in Directory.GetDirectories(Path.Combine(this._Constants.GetDataFolder(), "CameraData")))
            {
                string cameraId = new DirectoryInfo(folder).Name;
                List<string> list = new List<string>();
                string recordingsFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", cameraId, "Recordings");
                foreach (string file in Directory.GetFiles(folder))
                {
                    list.Add(file);
                }
                result.Add(cameraId, list);
            }
            return result;
        }

        public void RemoveVideo(string cameraId, string filename)
        {
            string fullPath = Path.Combine(this._Constants.GetDataFolder(), "CameraData", cameraId, "Recordings", filename);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                throw new BadRequestException($"File \"{filename}\" does not exist for camera \"{cameraId}\".");
            }
        }

        public byte[] GetPreviewOfVideo(string cameraId, string filename)
        {
            throw new NotImplementedException();
        }

        public byte[] GetVideo(string cameraId, string filename)
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
