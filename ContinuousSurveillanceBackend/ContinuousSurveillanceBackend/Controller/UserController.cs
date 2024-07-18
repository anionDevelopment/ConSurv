using ContinuousSurveillanceBackend.Core.Constants;
using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContinuousSurveillanceBackend.Core.Controller
{
    [ApiController]
    [Route(UserController.ControllerRoute)]
    public class UserController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(UserController)}";
        private readonly IGeneralLogger _Logger;
        private readonly IPersistence _Persistence;
        public UserController(IGeneralLogger logger, IPersistence persistence)
        {
            this._Logger = logger;
            this._Persistence = persistence;
        }

        [Authorize(CodeUnitSpecificConstants.UserGroupAdmin)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route($"{nameof(CreateUser)}")]
        public IActionResult CreateUser([FromHeader] string user, [FromHeader] string password)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route($"{nameof(Login)}")]
        public IActionResult Login([FromHeader] string user, [FromHeader] string password)
        {
            return Ok("sometoken");//TODO
        }

        [Authenticate]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route($"{nameof(Logout)}")]
        public IActionResult Logout([FromHeader] string token)
        {
            throw new NotImplementedException();
        }
    }
}
