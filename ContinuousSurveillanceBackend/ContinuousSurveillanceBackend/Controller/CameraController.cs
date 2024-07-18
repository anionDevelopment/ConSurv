using ContinuousSurveillanceBackend.Core.Constants;
using ContinuousSurveillanceBackend.Core.Model.DTOs;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
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
            throw new NotImplementedException();
        }

        [Authorize(CodeUnitSpecificConstants.UserGroupCameraManagers)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(CreateCamera)}")]
        public IActionResult CreateCamera([FromBody] CreateCameraDTO createCameraDTO)
        {
            this._CameraService.CreateCamera(createCameraDTO.Name, new NoRecording());
            return this.Ok();
        }

        [Authorize(CodeUnitSpecificConstants.UserGroupCameraManagers)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(RemoveCamera)}/{{{nameof(cameraId)}}}")]
        public IActionResult RemoveCamera([FromRoute] string cameraId)
        {
            this._CameraService.RemoveCamera(cameraId);
            return this.Ok();
        }

        [Authorize(CodeUnitSpecificConstants.UserGroupCameraManagers)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(UpdateCamera)}")]
        public IActionResult UpdateCamera([FromBody] UpdateCameraDTO createCameraDTO)
        {
            this._CameraService.UpdateCamera(createCameraDTO.Name, createCameraDTO.RecordMode);
            return this.Ok();
        }
    }
}
