using ConSurvBackend.Core.Model.RecordingModes;
using ConSurvBackend.Core.Miscellaneous;
using ConSurvBackend.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ConSurvBackend.Core.Configuration;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Exceptions;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.CommonDBTypes;

namespace ConSurvBackend.Core.Services
{
    public class CameraService : ICameraService
    {
        private static readonly object _LockObject = new object();
        private readonly IGRYLog _Log;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly IAuthenticationService<User> _AuthenticationService;
        private readonly IPersistence _Persistence;
        private readonly IDictionary<string, Camera> _Cameras = new Dictionary<string, Camera>();
        private readonly ITimeService _TimeService;
        private readonly IRTSPManager _RTSPManager;
        public CameraService(IPersistence persistence, IGRYLog log, IRTSPManager rtspManager, ITimeService timeService, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, IAuthenticationService<User> authenticationService)
        {
            this._Persistence = persistence;
            this._Log = log;
            this._RTSPManager = rtspManager;
            this._TimeService = timeService;
            this._AuthenticationService = authenticationService;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            //TODO load persisted cameras and start recording if necessary
        }
        public string CreateCamera(string name)
        {
            Camera camera = new Camera(Guid.NewGuid().ToString(), name, this._Log, this._RTSPManager, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration);
            this._Cameras[camera.Id] = camera;
            this._Persistence.CreateCamera(camera);
            return camera.Id;
        }

        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand)
        {
            Camera camera = this.GetCameraById(cameraId);
            if (camera.IsONVIFCamera)
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
            Camera camera = this.GetCameraById(cameraId);
            camera.EnsureIsNotRecording();
            this._Persistence.RemoveCamera(cameraId);
        }

        public void UpdateCamera(string cameraId, string name, RecordMode recordMode)
        {
            Camera camera = this.GetCameraById(cameraId);
            camera.Name = name;
            camera.RecordingMode = recordMode;
            this._Persistence.UpdateCamera(camera);
        }

        public Camera GetCameraById(string cameraId)
        {
            if (this._Cameras.TryGetValue(cameraId, out Camera? value))
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
            if (this._Cameras.Count == 0)
            {
                return 0;
            }
            else
            {
                return this._Cameras.Where(kvp => kvp.Value.IsAvailable()).Count() / this._Cameras.Count;
            }
        }

        public string Register(string username, string password)
        {
            lock (_LockObject)
            {
                User newUser = User.CreateNewUser(username, password == null ? null : this._AuthenticationService.Hash(password), this._TimeService);
                this._AuthenticationService.AddUserTyped(newUser);
                return newUser.Id;
            }
        }

        public bool UserWithNameExists(string username)
        {
            return this._Persistence.UserWithNameExists(username);
        }

        public IEnumerable<Camera> GetAllCameras()
        {
            return this._Cameras.Values;
        }
    }
}
