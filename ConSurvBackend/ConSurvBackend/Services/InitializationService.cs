using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.RecordModes;
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

namespace ConSurvBackend.Core.Services
{
    public class InitializationService : IInitializationService<CommandlineParameter>
    {
        private readonly IAuthenticationService _AuthenticationService;
        private readonly ICameraService _CameraService;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        private readonly IGeneralLogger _GeneralLogger;
        private readonly IExampleDataCreator _ExampleDataCreator;

        public InitializationService(IAuthenticationService authenticationService, IGeneralLogger generalLogger, ICameraService cameraService, IApplicationConstants<CodeUnitSpecificConstants> constants, IExampleDataCreator exampleDataCreator)
        {
            this._AuthenticationService = authenticationService;
            this._Constants = constants;
            this._CameraService = cameraService;
            this._GeneralLogger = generalLogger;
            this._ExampleDataCreator = exampleDataCreator;
        }

        public void Initialize(CommandlineParameter commandlineParameter)
        {
            this._GeneralLogger.Log("Initialize service...", Microsoft.Extensions.Logging.LogLevel.Information);
            this.EnsureBusinessLogicIsInitialized(commandlineParameter);
            this._GeneralLogger.Log("Service is initialized.", Microsoft.Extensions.Logging.LogLevel.Information);
        }

        private void EnsureBusinessLogicIsInitialized(CommandlineParameter commandlineParameter)
        {
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
                _CameraService.EnsureUserHasRole(adminUserId, adminsRole.Id);

                this._GeneralLogger.Log("Add initial cameras...", Microsoft.Extensions.Logging.LogLevel.Information);
                if (commandlineParameter.InitialCameraAddresses != null)
                {
                    uint counter = 0;
                    foreach (var initialCameraAddress in commandlineParameter.InitialCameraAddresses.OrderBy(x => x))
                    {
                        counter = counter + 1;
                        var cameraId = this._CameraService.CreateCamera($"Camera{counter.ToString().PadLeft(2, '0')}", initialCameraAddress);
                        var camera = this._CameraService.GetCameraById(cameraId);
                        camera.RecordMode = new RecordAlways();
                        this._CameraService.UpdateCamera(camera);
                    }
                }

                if (this._Constants.Environment is Development)
                {
                    this.StartWatchDogProcess(this._Constants.GetConfigurationFolder());
                    this._ExampleDataCreator.AddExampleData();
                }
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
