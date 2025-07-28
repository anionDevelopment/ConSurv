using ConSurvBackend.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Exceptions;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.RecordStates;
using ConSurvBackend.Core.Model.DTOs;
using Microsoft.Extensions.Logging;
using GRYLibrary.Core.Crypto;

namespace ConSurvBackend.Core.Services
{
    public class BusinessLogicService : IBusinessLogicService
    {
        private static readonly object _LockObject = new object();
        private readonly IGRYLog _Log;
        private readonly IAuthenticationService<User> _AuthenticationService;
        private readonly IPersistence _Persistence;
        private readonly ITimeService _TimeService;
        private readonly IRTSPManager _RTSPManager;
        private readonly IAuditLog _AuditLog;
        private readonly IRandomnessProvider _RandomnessProvider;
        private readonly IStreamOrganizerService _StreamOrganizerService;

        public BusinessLogicService(IPersistence persistence, IGRYLog log, IRTSPManager rtspManager, ITimeService timeService, IAuthenticationService<User> authenticationService, IRandomnessProvider randomnessProvider, IAuditLog auditLog, IStreamOrganizerService streamOrganizerService)
        {
            this._Persistence = persistence;
            this._Log = log;
            this._RTSPManager = rtspManager;
            this._TimeService = timeService;
            this._AuthenticationService = authenticationService;
            this._RandomnessProvider = randomnessProvider;
            this._AuditLog = auditLog;
            this._StreamOrganizerService = streamOrganizerService;
            //TODO load persisted cameras and start recording if necessary
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

        private string GetId(string seed)
        {
            return GRYLibrary.Core.Misc.Utilities.ByteArrayToHexString(new SHA256().Hash(GRYLibrary.Core.Misc.Utilities.StringToByteArray(Misc.Utilities.EscapeBasicAuthPasswords(seed)))).Substring(0, 6);
        }

        public bool IsAvailable(Camera camera)
        {
            return this.GetCurrentRecordingInformation(camera) is not Unavailable;
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
            camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RTSPManager));
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
                return this.GetAllCameras().Where(kvp => this.IsAvailable(kvp.Value)).Count() / this.GetAllCameras().Count;
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
            this._AuthenticationService.EnsureUserHasRole(userId, roleId);
            this._AuditLog.AuditLogger.Log($"Role {roleId} has been assigned to user {userId}.", LogLevel.Information);//TODO add information about why and by whom this was done
        }

        public void EnsureUserDoesNotHaveRole(string userId, string roleId)
        {
            this._AuthenticationService.EnsureUserDoesNotHaveRole(userId, roleId);
            this._AuditLog.AuditLogger.Log($"Role {roleId} has been unassigned to user {userId}.", LogLevel.Information);//TODO add information about why and by whom this was done
        }
    }
}
