using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService;

namespace ConSurvBackend.Core.Controller
{
    [ApiController]
    [Route(UserController.ControllerRoute)]
    public class UserController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(UserController)}";
        private readonly IGeneralLogger _Logger;
        private readonly IPersistence _Persistence;
        private readonly IAuthenticationService _AuthenticationService;
        private readonly ITimeService _TimeService;
        public UserController(IGeneralLogger logger, IPersistence persistence, IAuthenticationService authenticationService, ITimeService timeService)
        {
            this._Logger = logger;
            this._Persistence = persistence;
            this._AuthenticationService = authenticationService;
            this._TimeService = timeService;
        }

        [Authorize(CodeUnitSpecificConstants.UserGroupAdmin)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(CreateUser))]
        public IActionResult CreateUser([FromHeader] string user, [FromHeader] string password)
        {
            User user2 = GRYLibrary.Core.APIServer.CommonDBTypes.User.CreateNewUser(user, this._AuthenticationService.Hash(password),  this._TimeService);
            this._AuthenticationService.AddUser(user2);
            this._AuthenticationService.EnsureUserHasRole(user2.Id, this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.UserNameAdmin).Id);
            return this.Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccessToken))]
        [Route(nameof(Login))]
        public IActionResult Login([FromHeader] string user, [FromHeader] string password) => this.Ok(this._AuthenticationService.Login(user, password));

        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(Logout))]
        public IActionResult Logout()
        {
            this._AuthenticationService.Logout(this.User);
            return this.Ok();
        }

        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string[]))]
        [Route(nameof(GetRoles))]
        public IActionResult GetRoles() => this.Ok(this._AuthenticationService.GetRoles(this.User));
    }
}
