using ConSurvBackend.Core.Constants;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService _AuthenticationService;
        private readonly ITimeService _TimeService;
        private readonly ICameraService _CameraService;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        private readonly IGeneralLogger _GeneralLogger;
        private readonly IExampleDataCreator _ExampleDataCreator;

        public InitializationService(GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService authenticationService, IGeneralLogger generalLogger, ICameraService cameraService, ITimeService timeService, IApplicationConstants<CodeUnitSpecificConstants> constants, IExampleDataCreator exampleDataCreator)
        {
            this._AuthenticationService = authenticationService;
            this._TimeService = timeService;
            this._Constants = constants;
            this._CameraService = cameraService;
            this._GeneralLogger = generalLogger;
            this._ExampleDataCreator = exampleDataCreator;
        }

        public void Initialize()
        {
            string adminUsername = CodeUnitSpecificConstants.UsernameAdmin;
            if (!this._CameraService.UserWithNameExists(adminUsername))
            {
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

                string initialAdminPassword = CodeUnitSpecificConstants.UsernameAdmin;//only initial password. must be changed on production
                string adminUserId = this._CameraService.Register(adminUsername, initialAdminPassword);
                this._AuthenticationService.EnsureUserHasRole(adminUserId, adminsRole.Id);

                if (this._Constants.Environment is Development)
                {
                    this._ExampleDataCreator.AddExampleData();
                }
            }
            this._GeneralLogger.Log("Service is initialized.", Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }
}
