using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;

namespace ConSurvBackend.Core.Services
{
    public class ExampleDataCreator : IExampleDataCreator
    {
        private readonly IBusinessLogicService _CameraService;
        private readonly IPersistence _Persistence;
        private readonly IAuthenticationService<User> _AuthenticationService;
        private readonly ITimeService _TimeService;
        private readonly IGeneralLogger _Logger;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _Configuration;
        private static readonly Random _Random = new Random();

        public ExampleDataCreator(IPersistence persistence, IAuthenticationService<User> authenticationService, ITimeService timeService, IServerLog log, IApplicationConstants<CodeUnitSpecificConstants> constants, IBusinessLogicService cameraService, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> configuration)
        {
            this._Persistence = persistence;
            this._AuthenticationService = authenticationService;
            this._TimeService = timeService;
            this._Logger = log.Logger;
            this._Constants = constants;
            this._CameraService = cameraService;
            this._Configuration = configuration;
        }

        public void AddExampleData()
        {
            string moderatorUserId = this._CameraService.Register("moderator", "moderator");
            Role moderatorRole = this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.RolenameModerators);
            this._AuthenticationService.EnsureUserHasRole(moderatorUserId, moderatorRole.Id);

            this._CameraService.Register("user01", "user01");
            this._CameraService.Register("user02", "user02");

            //TODO add more example data
        }
    }
}
