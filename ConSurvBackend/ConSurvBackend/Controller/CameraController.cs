using ConSurvBackend.Core.BackgroundServices;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IBusinessLogicService _CameraService;
        private readonly IPreviewService _PreviewService;
        public CameraController(IGeneralLogger logger, IPersistence persistence, IBusinessLogicService cameraService, IPreviewService previewService)
        {
            this._Logger = logger;
            this._Persistence = persistence;
            this._CameraService = cameraService;
            this._PreviewService = previewService;
        }

        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(byte[]))]
        [Route($"{nameof(GetPreview)}/{{{nameof(cameraId)}}}/{{{nameof(maximalHeight)}}}/{{{nameof(maximalWidth)}}}")]
        public IActionResult GetPreview([FromRoute] string cameraId, [FromRoute] uint? maximalHeight, [FromRoute] uint? maximalWidth)
        {
            uint height = maximalHeight.HasValue ? maximalHeight.Value : 640;
            uint width = maximalWidth.HasValue ? maximalWidth.Value : 360;
            return this.Ok(Misc.Utilities.ResizeImage(this._PreviewService.GetPreview(cameraId), height, width));
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

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route(nameof(ListCameras))]
        public IActionResult ListCameras()
        {
            throw new System.NotImplementedException();
        }

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(ListAvailableVideos)}/{{{nameof(cameraId)}}}")]
        public IActionResult ListAvailableVideos([FromRoute] string cameraId)
        {
            throw new System.NotImplementedException();
        }

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(DownloadVideo)}/{{{nameof(cameraId)}}}/{{{nameof(filename)}}}")]
        public IActionResult DownloadVideo([FromRoute] string cameraId, [FromRoute]string filename)
        {
            throw new System.NotImplementedException();
        }

        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(RemoveVideo)}/{{{nameof(cameraId)}}}/{{{nameof(filename)}}}")]
        public IActionResult RemoveVideo([FromRoute] string cameraId, [FromRoute] string filename)
        {
            throw new System.NotImplementedException();
        }

    }
}
