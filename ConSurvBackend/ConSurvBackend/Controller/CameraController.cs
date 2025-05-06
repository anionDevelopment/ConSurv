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
        public IActionResult GetStream([FromRoute] string cameraId)
        {
            throw new NotImplementedException();
            //see https://stackoverflow.com/a/69986391/3905529
            // or https://stackoverflow.com/questions/2245040/how-can-i-display-an-rtsp-video-stream-in-a-web-page
            // or https://stackoverflow.com/questions/1735933/streaming-via-rtsp-or-rtp-in-html5
            // or https://stackoverflow.com/questions/31948604/handling-receiving-live-video-webcam-stream-from-webrtc-or-any-browser-based-c
            // or https://stackoverflow.com/questions/71034124/how-to-serve-video-file-stream-from-asp-net-core-6-minimal-api
            // or https://stackoverflow.com/questions/31766623/stream-video-content-through-web-api-2
            // or https://stackoverflow.com/questions/49618810/net-core-2-0-web-api-for-video-streaming-from-filestream?rq=3
            // or https://stackoverflow.com/questions/23011302/best-approach-to-get-rtsp-streaming-into-web-browser-from-ip-camera
            // or https://stackoverflow.com/questions/26999595/what-steps-are-needed-to-stream-rtsp-from-ffmpeg
            // or https://stackoverflow.com/questions/69899709/forwarding-rtsp-stream-from-ip-camera-to-browser-in-asp-net-core
            // or https://github.com/sipsorcery-org/sipsorcery/issues/408
            // or https://stackoverflow.com/questions/4241992/video-streaming-over-websockets-using-javascript
            // or https://anzal.hashnode.dev/a-beginners-guide-to-rtsp-streaming-with-websockets-using-nodejs-and-ffmpeg
            // or https://stackoverflow.com/questions/64934740/websocket-connection-with-authorization-header

        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(byte[]))]
        [Route($"{nameof(GetPreview)}/{{{nameof(cameraId)}}}/{{{nameof(maximalHeight)}}}/{{{nameof(maximalWidth)}}}")]
        public IActionResult GetPreview([FromRoute] string cameraId, [FromRoute] uint? maximalHeight, [FromRoute] uint? maximalWidth)
        {
            return this.Ok(this._CameraService.GetPreview(this._CameraService.GetCameraById(cameraId), maximalHeight, maximalWidth));
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route(nameof(CreateCamera))]
        public IActionResult CreateCamera()
        {
            return this.Ok(this._CameraService.CreateCamera("New camera", "rtsp://mycamera.example.com/stream"));
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route($"{nameof(RemoveCamera)}/{{{nameof(cameraId)}}}")]
        public IActionResult RemoveCamera([FromRoute] string cameraId)
        {
            this._CameraService.RemoveCamera(cameraId);
            return this.Ok();
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [Route(nameof(UpdateCamera))]
        public IActionResult UpdateCamera([FromBody] UpdateCameraDTO updateCameraDTO)
        {
            this._CameraService.UpdateCamera(updateCameraDTO.ToCamera());
            return this.Ok();
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO))]
        [Route($"{nameof(Camera)}/{{{nameof(cameraId)}}}")]
        public IActionResult Camera([FromRoute] string cameraId)
        {
            return this.Ok(this._CameraService.ToDTO(this._CameraService.GetCameraById(cameraId)));
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO[]))]
        [Route(nameof(Cameras))]
        public IActionResult Cameras()
        {
            System.Collections.Generic.List<CameraDTO> result = this._CameraService.GetAllCameras().Select(camera => this._CameraService.ToDTO(camera.Value)).ToList();
            return this.Ok(result);
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
