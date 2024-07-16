using ContinuousSurveillanceBackend.Core.Constants;
using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(GetStream)}/{{{nameof(cameraId)}}}")]
        public IActionResult GetStream([FromRoute] string cameraId, [FromQuery] string resolution)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(CreateCamera)}")]
        public IActionResult CreateCamera([FromBody] CreateCameraDTO createCameraDTO)
        {
            _CameraService.CreateCamera(createCameraDTO.Name, createCameraDTO.CameraAddress, new NotRecording());
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(RemoveCamera)}/{{{nameof(cameraId)}}}")]
        public IActionResult RemoveCamera([FromRoute] string cameraId)
        {
            _CameraService.RemoveCamera(cameraId);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(UpdateCamera)}")]
        public IActionResult UpdateCamera([FromBody] UpdateCameraDTO createCameraDTO)
        {
            _CameraService.UpdateCamera(createCameraDTO.Name, createCameraDTO.CameraAddress, createCameraDTO.RecordMode);
            return Ok();
        }
    }
}
