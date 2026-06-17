using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public UserController(IServerLog logger, IPersistence persistence, IAuthenticationService authenticationService, ITimeService timeService)
        {
            this._Logger = logger.Logger;
            this._Persistence = persistence;
            this._AuthenticationService = authenticationService;
            this._TimeService = timeService;
        }

        /// <summary>
        /// Authenticates a user with the supplied credentials and returns a new access token on success.
        /// </summary>
        /// <param name="user">The username, provided via the <c>x-user</c> HTTP header.</param>
        /// <param name="password">The plain-text password, provided via the <c>x-password</c> HTTP header.</param>
        /// <returns>
        /// 200 OK with an <see cref="AccessToken"/> on successful authentication;
        /// 400 Bad Request if the username or password header is missing;
        /// 500 Internal Server Error on an unexpected authentication failure.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccessToken))]
        [Route(nameof(Login))]
        public IActionResult Login([FromHeader(Name = "x-user")] string? user, [FromHeader(Name = "x-password")] string? password)
        {
            try
            {
                if (user == null)
                {
                    return this.BadRequest("user not given");
                }
                if (password == null)
                {
                    return this.BadRequest("password not given");
                }
                return this.Ok(this._AuthenticationService.Login(user, password));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// Creates a new user account with the given credentials and assigns the admin role to it.
        /// Requires the caller to hold the admin role.
        /// </summary>
        /// <param name="user">The desired username, provided via request header.</param>
        /// <param name="password">The plain-text password that will be hashed before storage, provided via request header.</param>
        /// <returns>200 OK when the user has been created and the admin role assigned successfully.</returns>
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

        /// <summary>
        /// Invalidates the access token of the currently authenticated user, effectively logging them out.
        /// </summary>
        /// <returns>
        /// 200 OK when the token has been revoked;
        /// 401 Unauthorized if no valid access token is present in the current request context.
        /// </returns>
        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(Logout))]
        public IActionResult Logout()
        {
            if ((!this.HttpContext.Items.ContainsKey(AuthenticationMiddleware.CurrentlyUsedAccessTokenInformationName)) ||
                (this.HttpContext.Items[AuthenticationMiddleware.CurrentlyUsedAccessTokenInformationName] == null))
            {
                return this.StatusCode(StatusCodes.Status401Unauthorized);
            }
            string currentlyUsedAccessToken = (string)this.HttpContext.Items[AuthenticationMiddleware.CurrentlyUsedAccessTokenInformationName]!;
            this._AuthenticationService.Logout(currentlyUsedAccessToken);
            return this.Ok();
        }

        /// <summary>
        /// Checks whether the supplied access token is still valid (not expired and not revoked).
        /// </summary>
        /// <param name="accessToken">The access token to validate, provided via request header.</param>
        /// <returns>200 OK with <c>true</c> if the token is valid, or <c>false</c> otherwise.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [Route(nameof(TokenIsValid))]
        public IActionResult TokenIsValid([FromHeader] string accessToken)
        {
            bool result = this._AuthenticationService.AccessTokenIsValid(accessToken);
            this._Logger.Log($"Checked if access token {accessToken} is valid. Result: {result}");
            return this.Ok(result);
        }

        /// <summary>
        /// Returns the list of role names assigned to the currently authenticated user.
        /// </summary>
        /// <returns>200 OK with an array of role name strings.</returns>
        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string[]))]
        [Route(nameof(GetRoles))]
        public IActionResult GetRoles()
        {
            return this.Ok(this.GetUser().Roles);
        }

        /// <summary>
        /// Returns profile information (e.g., username, roles) for the currently authenticated user.
        /// </summary>
        /// <returns>200 OK with a <see cref="UserInformationDTO"/> for the current user.</returns>
        [Authenticate]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInformationDTO))]
        [Route(nameof(GetUserInformation))]
        public IActionResult GetUserInformation()
        {
            return this.Ok(Utilities.GetUserInformation(this.GetUser()));
        }

        //TODO there must be functions for an admin to grant the read-right (=>role: user) or the update-right (=>role: moderator) to certain cameras only.

        private User GetUser()
        {
            return Tools.GetUser(this.User, this._AuthenticationService);
        }
    }
}
