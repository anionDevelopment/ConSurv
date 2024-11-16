using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordingModes;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [Authorize(CodeUnitSpecificConstants.UserGroupUser)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(GetStream)}/{{{nameof(cameraId)}}}")]
        public IActionResult GetStream([FromRoute] string cameraId, [FromQuery] string resolution)
        {
            throw new NotImplementedException();//see https://stackoverflow.com/a/69986391/3905529
        }

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route($"{nameof(CreateCamera)}")]
        public IActionResult CreateCamera([FromBody] CreateCameraDTO createCameraDTO)
        {
            return this.Ok(this._CameraService.CreateCamera(createCameraDTO.Name, new NoRecording()));
        }

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route($"{nameof(RemoveCamera)}/{{{nameof(cameraId)}}}")]
        public IActionResult RemoveCamera([FromRoute] string cameraId)
        {
            this._CameraService.RemoveCamera(cameraId);
            return this.Ok();
        }

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route($"{nameof(UpdateCamera)}")]
        public IActionResult UpdateCamera([FromBody] UpdateCameraDTO updateCameraDTO)
        {
            this._CameraService.UpdateCamera(updateCameraDTO.CameraId, updateCameraDTO.Name, updateCameraDTO.RecordMode);
            return this.Ok();
        }

        #region ONVIF-specific

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(RunONVIFCommand)}/{nameof(cameraId)}")]
        public IActionResult RunONVIFCommand([FromRoute] string cameraId, [FromBody] ONVIFCommand onvifCommand)
        {
            this._CameraService.RunONVIFCommand(cameraId, onvifCommand);
            return this.Ok();
        }

        #endregion

    }
}
