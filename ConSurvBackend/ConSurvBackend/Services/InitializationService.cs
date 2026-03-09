using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConSurvBackend.Core.Services
{
    public class InitializationService : IInitializationService<CommandlineParameter>
    {
        private readonly IAuthenticationService _AuthenticationService;
        private readonly IBusinessLogicService _CameraService;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        private readonly IGeneralLogger _GeneralLogger;
        private readonly IExampleDataCreator _ExampleDataCreator;
        private readonly IPersistence _Persistence;
        private InitializationState _InitializationState = new Uninitialized();
        public InitializationService(IAuthenticationService authenticationService, IGeneralLogger generalLogger, IBusinessLogicService cameraService, IApplicationConstants<CodeUnitSpecificConstants> constants, IExampleDataCreator exampleDataCreator, IPersistence persistence)
        {
            this._AuthenticationService = authenticationService;
            this._Persistence = persistence;
            this._Constants = constants;
            this._CameraService = cameraService;
            this._GeneralLogger = generalLogger;
            this._ExampleDataCreator = exampleDataCreator;
        }

        public InitializationState GetInitializationState()
        {
            return this._InitializationState;
        }

        public void Initialize(CommandlineParameter commandlineParameter)
        {
            this._InitializationState = new Initializing();
            try
            {
                this._GeneralLogger.Log("Initialize service...", Microsoft.Extensions.Logging.LogLevel.Information);
                if (this._Persistence is IInitializable initializablePersistence)
                {
                    initializablePersistence.WaitUntilAvailable(TimeSpan.FromMinutes(2));
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
                    moderatorsRole.DirectlyInheritedRoles = new HashSet<Role>();
                    moderatorsRole.DirectlyInheritedRoles.Add(usersRole);
                    this._AuthenticationService.UpdateRole(moderatorsRole);

                    this._AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.RolenameAdmins);
                    Role adminsRole = this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.RolenameAdmins);
                    adminsRole.DirectlyInheritedRoles = new HashSet<Role>();
                    adminsRole.DirectlyInheritedRoles.Add(moderatorsRole);
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

                    if (commandlineParameter.RealRun)
                    {
                        if (!ConSurvBackend.Core.Misc.Utilities.IsRunningInContainer())
                        {
                            this.StartWatchDogProcess(this._Constants.GetConfigurationFolder());
                        }
                        this._ExampleDataCreator.AddExampleData();
                    }
                }

                this._GeneralLogger.Log("Service is initialized.", Microsoft.Extensions.Logging.LogLevel.Information);
                this._InitializationState = new Initialized();
            }
            catch
            {
                this._InitializationState = new InitializationFailed();
                throw;
            }
        }

        private void StartWatchDogProcess(string configurationFolder)
        {
            this._GeneralLogger.Log($"Start watch-dog-process in {configurationFolder}...", Microsoft.Extensions.Logging.LogLevel.Information);
            string processesFileName = "StartedProcesses.txt";
            string processesFile = Path.Combine(configurationFolder, processesFileName);
            GRYLibrary.Core.Misc.Utilities.RunEspoc(processesFile, null);
        }
    }
}
