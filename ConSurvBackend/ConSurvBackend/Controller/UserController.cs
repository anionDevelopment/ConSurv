using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Miscellaneous;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.MidT.Auth;
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

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccessToken))]
        [Route(nameof(Login))]
        public IActionResult Login([FromHeader] string user, [FromHeader] string password)
        {
            return this.Ok(this._AuthenticationService.Login(user, password));
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameAdmins)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(CreateUser))]
        public IActionResult CreateUser([FromHeader] string user, [FromHeader] string password)
        {
            User typedUser = GRYLibrary.Core.APIServer.CommonDBTypes.User.CreateNewUser(user, this._AuthenticationService.Hash(password), this._TimeService);
            this._AuthenticationService.AddUser(typedUser);
            this._AuthenticationService.EnsureUserHasRole(typedUser.Id, this._AuthenticationService.GetRoleByName(CodeUnitSpecificConstants.UsernameAdmin).Id);
            return this.Ok();
        }

        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(Logout))]
        public IActionResult Logout()
        {
            string currentlyUsedAccessToken = (string)this.HttpContext.Items[AuthenticationMiddleware.CurrentlyUsedAccessTokenInformationName];
            this._AuthenticationService.Logout(currentlyUsedAccessToken);
            return this.Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Route(nameof(TokenIsValid))]
        public IActionResult TokenIsValid([FromHeader] string accessToken)
        {
            return this.Ok(this._AuthenticationService.AccessTokenIsValid(accessToken));
        }

        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string[]))]
        [Route(nameof(GetRoles))]
        public IActionResult GetRoles()
        {
            return this.Ok(this.GetUser().Roles);
        }

        [Authenticate]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInformationDTO))]
        [Route(nameof(GetUserInformation))]
        public IActionResult GetUserInformation()
        {
            return this.Ok(Utilities.GetUserInformation(this.GetUser()));
        }

        private User GetUser()
        {
            return Tools.GetUser(this.User, this._AuthenticationService);
        }
    }
}
