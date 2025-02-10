using ConSurvBackend.Core.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using ConSurvBackend.Core.Configuration;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Exceptions;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.RecordStates;
using ConSurvBackend.Core.Model.DTOs;

namespace ConSurvBackend.Core.Services
{
    public class CameraService : ICameraService
    {
        private static readonly object _LockObject = new object();
        private readonly IGRYLog _Log;
        private readonly IAuthenticationService<User> _AuthenticationService;
        private readonly IPersistence _Persistence;
        private readonly ITimeService _TimeService;
        private readonly IRTSPManager _RTSPManager;
        private readonly IRandomnessProvider _RandomnessProvider;
        public CameraService(IPersistence persistence, IGRYLog log, IRTSPManager rtspManager, ITimeService timeService,  IAuthenticationService<User> authenticationService, IRandomnessProvider randomnessProvider)
        {
            this._Persistence = persistence;
            this._Log = log;
            this._RTSPManager = rtspManager;
            this._TimeService = timeService;
            this._AuthenticationService = authenticationService;
            this._RandomnessProvider = randomnessProvider;
            //TODO load persisted cameras and start recording if necessary
        }
        public string CreateCamera(string name, string streamURL)
        {
            Camera camera = new Camera(this.GetId(), name);
            camera.VideoInformation.StreamURL = streamURL;
            this.GetAllCameras()[camera.Id] = camera;
            this._Persistence.CreateCamera(camera);
            _Log.Log($"Created camera {camera.Id}.");
            return camera.Id;
        }

        private string GetId()
        {
            return GRYLibrary.Core.Misc.Utilities.GetRandomAlphaHexCharacter(this._RandomnessProvider) + GRYLibrary.Core.Misc.Utilities.GetRandomHexCharacter(5, this._RandomnessProvider);
        }

        public byte[] GetPreview(Camera camera)
        {
            //TODO check permission
            return camera.VideoInformation.GetPreview(camera, this._RTSPManager);
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
        }

        public void UpdateCamera(Camera camera)
        {
            //TODO check permission
            this._Persistence.UpdateCamera(camera);
            camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RTSPManager));
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
    }
}
