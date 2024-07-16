using ContinuousSurveillanceBackend.Core.Constants;
using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContinuousSurveillanceBackend.Core.Controller
{
    [ApiController]
    [Route(ExampleController.ControllerRoute)]
    public class ExampleController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(ExampleController)}";
        private readonly IGeneralLogger _Logger;
        private readonly IPersistence _Persistence;
        public ExampleController(IGeneralLogger logger, IPersistence persistence)
        {
            this._Logger = logger;
            this._Persistence = persistence;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        [Route($"Calculator/{{{nameof(function)}}}")]
        public IActionResult Calculate([FromRoute] string function, [FromQuery] decimal parameter1, [FromQuery] decimal parameter2)
        {
            return Ok(parameter1 + parameter2);//TODO
        }
    }
}
