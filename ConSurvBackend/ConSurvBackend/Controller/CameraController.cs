using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        private readonly IRuntimeData _RuntimeData;
        public CameraController(IServerLog logger, IPersistence persistence, IBusinessLogicService cameraService, IRuntimeData runtimeData)
        {
            this._Logger = logger.Logger;
            this._Persistence = persistence;
            this._CameraService = cameraService;
            this._RuntimeData = runtimeData;
        }

        /// <summary>
        /// Returns the latest preview image (as raw bytes) for the specified camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera whose preview should be retrieved.</param>
        /// <returns>200 OK with the JPEG/PNG preview bytes.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(byte[]))]
        [Route($"{nameof(GetPreview)}/{{{nameof(cameraId)}}}")]
        public IActionResult GetPreview([FromRoute] string cameraId)
        {
            return this.Ok(this._RuntimeData.GetLatestPreview(cameraId).Data);
        }

        /// <summary>
        /// Creates a new camera with default name and RTSP address and returns its generated identifier.
        /// </summary>
        /// <returns>200 OK with the new camera's unique identifier as a string.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route(nameof(CreateCamera))]
        public IActionResult CreateCamera()
        {
            return this.Ok(this._CameraService.CreateCamera("New camera", "rtsp://mycamera.example.com/stream"));
        }

        /// <summary>
        /// Permanently removes the specified camera and all associated data.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera to remove.</param>
        /// <returns>200 OK on success.</returns>
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

        /// <summary>
        /// Updates the properties of an existing camera using the values provided in the request body.
        /// </summary>
        /// <param name="updateCameraDTO">The DTO containing the updated camera properties.</param>
        /// <returns>200 OK on success.</returns>
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

        /// <summary>
        /// Retrieves the full configuration details of a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera to retrieve.</param>
        /// <returns>200 OK with a <see cref="CameraDTO"/> representing the camera.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CameraDTO))]
        [Route($"{nameof(Camera)}/{{{nameof(cameraId)}}}")]
        public IActionResult Camera([FromRoute] string cameraId)
        {
            return this.Ok(this._CameraService.ToDTO(this._CameraService.GetCameraById(cameraId)));
        }

        /// <summary>
        /// Returns the list of all configured cameras visible to the current user.
        /// </summary>
        /// <returns>200 OK with an array of <see cref="CameraDTO"/> objects.</returns>
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

        /// <summary>
        /// Executes an ONVIF command on the specified camera (e.g., PTZ control, preset recall).
        /// </summary>
        /// <param name="cameraId">The unique identifier of the target camera.</param>
        /// <param name="onvifCommandDTO">The ONVIF command to execute, including its type and parameters.</param>
        /// <returns>200 OK on success.</returns>
        [Authenticate]
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

        /// <summary>
        /// Returns a dictionary mapping each camera identifier to the list of recorded video filenames available for that camera.
        /// </summary>
        /// <returns>200 OK with a dictionary of camera IDs to file name lists.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(ListVideos)}")]
        public IActionResult ListVideos()
        {
            IDictionary<string, IList<string>> result = this._CameraService.GetVideos();
            return this.Ok(result);
        }

        /// <summary>
        /// Returns a preview thumbnail image for the specified recorded video file of a camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera that owns the video.</param>
        /// <param name="filename">The filename of the recorded video for which the preview is requested.</param>
        /// <returns>200 OK with the thumbnail image bytes.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(GetPreviewOfVideo)}{{{nameof(cameraId)}}}/{{{nameof(filename)}}}")]
        public IActionResult GetPreviewOfVideo([FromRoute] string cameraId, [FromRoute] string filename)
        {
            byte[] content = this._CameraService.GetPreviewOfVideo(cameraId, filename);
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Downloads the raw bytes of a specific recorded video file for a given camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera that owns the video.</param>
        /// <param name="filename">The filename of the recorded video to download.</param>
        /// <returns>200 OK with the video file bytes.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(DownloadVideo)}/{{{nameof(cameraId)}}}/{{{nameof(filename)}}}")]
        public IActionResult DownloadVideo([FromRoute] string cameraId, [FromRoute] string filename)
        {
            byte[] content = this._CameraService.GetVideo(cameraId, filename);
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Permanently deletes a specific recorded video file belonging to the given camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera that owns the video.</param>
        /// <param name="filename">The filename of the recorded video to delete.</param>
        /// <returns>200 OK on success.</returns>
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameModerators)]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route($"{nameof(RemoveVideo)}/{{{nameof(cameraId)}}}/{{{nameof(filename)}}}")]
        public IActionResult RemoveVideo([FromRoute] string cameraId, [FromRoute] string filename)
        {
            this._CameraService.RemoveVideo(cameraId, filename);
            return this.Ok();
        }

    }
}
