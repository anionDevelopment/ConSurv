using ContinuousSurveillanceBackend.Core.Constants;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService _AuthenticationService;
        private readonly ITimeService _TimeService;

        public InitializationService(GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService authenticationService, ITimeService timeService)
        {
            this._AuthenticationService = authenticationService;
            this._TimeService = timeService;
        }

        public void Initialize()
        {
            if (!_AuthenticationService.UserExistsByName(CodeUnitSpecificConstants.UserNameAdmin))
            {
                _AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserGroupUser);
                Role userRole = _AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.UserGroupUser);

                _AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserGroupCameraManagers);
                Role cameraManagerRole = _AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.UserGroupCameraManagers);
                cameraManagerRole.InheritedRoles.Add(userRole);
                _AuthenticationService.UpdateRole(cameraManagerRole);

                _AuthenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserNameAdmin);
                Role adminRole = _AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.UserNameAdmin);
                adminRole.InheritedRoles.Add(cameraManagerRole);
                adminRole.InheritedRoles.Add(userRole);
                _AuthenticationService.UpdateRole(adminRole);

                string adminUserName = "admin";
                string password = adminUserName+"pw";//initial password

                User adminUser = User.CreateNewUser("admin", _AuthenticationService.Hash(password), _TimeService);
                _AuthenticationService.AddUser(adminUser);
                _AuthenticationService.EnsureUserHasRole(adminUser.Id, adminRole.Id);
            }
        }
    }
}
