using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ConSurvBackend.Core.Controller
{
    [ApiController]
    [Route(CameraController.ControllerRoute)]
    public class CameraController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(CameraController)}";
        private readonly IGeneralLogger _Logger;
        private readonly IPersistence _Persistence;
        private readonly ICameraService _CameraService;
        public CameraController(IGeneralLogger logger, IPersistence persistence, ICameraService cameraService)
        {
            this._Logger = logger;
            this._Persistence = persistence;
            this._CameraService = cameraService;
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(GetStream)}/{{{nameof(cameraId)}}}")]
        public IActionResult GetStream([FromRoute] string cameraId, [FromQuery] string resolution)
        {
            throw new NotImplementedException();//see https://stackoverflow.com/a/69986391/3905529
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route(nameof(CreateCamera))]
        public IActionResult CreateCamera()
        {
            return this.Ok(this._CameraService.CreateCamera("New camera"));
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route($"{nameof(RemoveCamera)}/{{{nameof(cameraId)}}}")]
        public IActionResult RemoveCamera([FromRoute] string cameraId)
        {
            this._CameraService.RemoveCamera(cameraId);
            return this.Ok();
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(UpdateCamera))]
        public IActionResult UpdateCamera([FromBody] UpdateCameraDTO updateCameraDTO)
        {
            this._CameraService.UpdateCamera(updateCameraDTO.CameraId, updateCameraDTO.Name, updateCameraDTO.RecordMode.ToRecordMode());
            return this.Ok();
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO))]
        [Route($"{nameof(Camera)}/{{{nameof(cameraId)}}}")]
        public IActionResult Camera([FromRoute] string cameraId)
        {
            return this.Ok(this._CameraService.GetCameraById(cameraId).ToDTO());
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO[]))]
        [Route(nameof(Cameras))]
        public IActionResult Cameras()
        {
            return this.Ok(this._CameraService.GetAllCameras().Select(camera => camera.ToDTO()));
        }

        #region ONVIF-specific

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(RunONVIFCommand)}/{{{nameof(cameraId)}}}")]
        public IActionResult RunONVIFCommand([FromRoute] string cameraId, [FromBody] ONVIFCommandDTO onvifCommandDTO)
        {
            this._CameraService.RunONVIFCommand(cameraId, onvifCommandDTO.ToONVIFCommand());
            return this.Ok();
        }

        #endregion

    }
}
