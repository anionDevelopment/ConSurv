using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Misc;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ConSurvBackend.Core.Services
{
    public class InitializationService : IInitializationService<CommandlineParameter>
    {
        private readonly IAuthenticationService _AuthenticationService;
        private readonly IBusinessLogicService _CameraService;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        private readonly IGeneralLogger _GeneralLogger;
        private readonly IExampleDataCreator _ExampleDataCreator;
        private readonly IRTSPManager _RTSPManager;
        private readonly IStreamOrganizerService _StreamOrganizerService;
        private readonly IPersistence _Persistence;

        public InitializationService(IAuthenticationService authenticationService, IGeneralLogger generalLogger, IBusinessLogicService cameraService, IApplicationConstants<CodeUnitSpecificConstants> constants, IExampleDataCreator exampleDataCreator, IRTSPManager rtspManager, IStreamOrganizerService streamOrganizerService, IPersistence persistence)
        {
            this._AuthenticationService = authenticationService;
            this._Persistence = persistence;
            this._Constants = constants;
            this._CameraService = cameraService;
            this._GeneralLogger = generalLogger;
            this._ExampleDataCreator = exampleDataCreator;
            this._RTSPManager = rtspManager;
            this._StreamOrganizerService = streamOrganizerService;
        }

        public void Initialize(CommandlineParameter commandlineParameter)
        {
            this._GeneralLogger.Log("Initialize service...", Microsoft.Extensions.Logging.LogLevel.Information);
            //if (_Persistence is IInitializable initializablePersistence)
            //{
            //    initializablePersistence.Initialize();
            //}
            if (_Persistence is IInitializable initializablePersistence)
            {
                initializablePersistence.Initialize();
            }
            string adminUsername = CodeUnitSpecificConstants.UsernameAdmin;
            if (!this._CameraService.UserWithNameExists(adminUsername))
            {
                this._GeneralLogger.Log("Add admin-user...", Microsoft.Extensions.Logging.LogLevel.Information);

                this._AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.RolenameUsers);
                Role usersRole = this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.RolenameUsers);

                this._AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.RolenameModerators);
                Role moderatorsRole = this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.RolenameModerators);
                moderatorsRole.InheritedRoles = new HashSet<Role>();
                moderatorsRole.InheritedRoles.Add(usersRole);
                this._AuthenticationService.UpdateRole(moderatorsRole);

                this._AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.RolenameAdmins);
                Role adminsRole = this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.RolenameAdmins);
                adminsRole.InheritedRoles = new HashSet<Role>();
                adminsRole.InheritedRoles.Add(moderatorsRole);
                this._AuthenticationService.UpdateRole(adminsRole);
                string initialAdminPassword = string.IsNullOrWhiteSpace(commandlineParameter.InitialAdminPassword) ? CodeUnitSpecificConstants.UsernameAdmin : commandlineParameter.InitialAdminPassword;//only initial password. should be changed as soon as possible by the admin of course.
                string adminUserId = this._CameraService.Register(adminUsername, initialAdminPassword);
                this._CameraService.EnsureUserHasRole(adminUserId, adminsRole.Id);

                this._GeneralLogger.Log("Add initial cameras...", Microsoft.Extensions.Logging.LogLevel.Information);
                if (commandlineParameter.InitialCameraAddresses != null)
                {
                    uint counter = 0;
                    foreach (string? initialCameraAddress in commandlineParameter.InitialCameraAddresses.OrderBy(x => x))
                    {
                        counter = counter + 1;
                        string cameraId = this._CameraService.CreateCamera($"Camera{counter.ToString().PadLeft(2, '0')}", initialCameraAddress);
                        Model.Base.Camera camera = this._CameraService.GetCameraById(cameraId);
                    }
                }

                if (this._Constants.Environment is Development)
                {
                    this.StartWatchDogProcess(this._Constants.GetConfigurationFolder());
                    this._ExampleDataCreator.AddExampleData();
                }
            }
            this.OrganizeCameras();
            this.StartCameras();
            this._GeneralLogger.Log("Service is initialized.", Microsoft.Extensions.Logging.LogLevel.Information);
        }

        private void OrganizeCameras()
        {
            this._StreamOrganizerService.InitializeCameraOrganization();
            foreach (Model.Base.Camera camera in this._CameraService.GetAllCameras().Values)
            {
                this._StreamOrganizerService.OrganizeCamera(camera);
            }
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        private void StartCameras()
        {
            foreach (Model.Base.Camera camera in this._CameraService.GetAllCameras().Values)
            {
                camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RTSPManager));
            }
        }

        private void StartWatchDogProcess(string configurationFolder)
        {
            this._GeneralLogger.Log("Start watch-dog-process...", Microsoft.Extensions.Logging.LogLevel.Information);
            int currentProcessId = Environment.ProcessId;
            Process process = new Process();
            process.StartInfo.FileName = "scespoc";
            process.StartInfo.Arguments = $"--processid {currentProcessId} --file ./StartedProcesses.txt";
            process.StartInfo.WorkingDirectory = configurationFolder;
            process.Start();
        }
    }
}
