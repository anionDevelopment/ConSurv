using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace ConSurvBackend.Core.Controller
{
    [ApiController]
    [Route(InsightsController.ControllerRoute)]
    public class InsightsController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(InsightsController)}";
        private readonly IProcessManager _ProcessManager;
        private readonly IApplicationConstants _ApplicationConstants;

        public InsightsController(IProcessManager processManager, IApplicationConstants applicationConstants)
        {
            this._ProcessManager = processManager;
            this._ApplicationConstants = applicationConstants;
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO))]
        [Route($"{nameof(GetRunningProcesses)}")]
        public IActionResult GetRunningProcesses()
        {
            return new JsonResult(_ProcessManager.GetRunningProcesses().Select(p => p.ToDTO()));
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO))]
        [Route($"{nameof(GetMediaMTXConfiguration)}/{nameof(cameraId)}")]
        public IActionResult GetMediaMTXConfiguration(string cameraId)
        {
            string mediaMTXConfigurationFolder = Path.Combine(this._ApplicationConstants.GetConfigurationFolder(), "MediaMTXConfigurationFiles");
            string mediaMTXConfigurationFile = Path.Combine(mediaMTXConfigurationFolder, $"MediaMTXConfiguration.{cameraId}.txt");
            return Ok(Misc.Utilities.EscapeBasicAuthPasswords(System.IO.File.ReadAllText(mediaMTXConfigurationFile, ConSurvBackend.Core.Misc.Utilities._Encoding)));
        }

    }
}
