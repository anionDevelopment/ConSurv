using ConSurvBackend.Core.Constants;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace ConSurvBackend.Core.Controller
{
    [ApiController]
    [Route(StreamingController.ControllerRoute)]
    public class StreamingController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(StreamingController)}";
        private readonly IApplicationConstants _ApplicationConstants;
        private readonly IGRYLog _Log;
        public StreamingController(IApplicationConstants applicationConstants, IServerLog log)
        {
            this._ApplicationConstants = applicationConstants;
            this._Log = log.Logger;
        }

        /// <summary>
        /// Serves an HLS stream segment or playlist file for the specified stream.
        /// Validates the filename format, resolves the file from the camera's fragment folder,
        /// and returns it with the appropriate MIME type (<c>application/vnd.apple.mpegurl</c> for
        /// <c>.m3u8</c> playlists, <c>video/MP2T</c> for <c>.ts</c> segments).
        /// </summary>
        /// <param name="streamId">The identifier of the stream (typically a camera ID).</param>
        /// <param name="filename">The HLS segment or playlist filename to serve (must match <c>^[0-9A-Za-z_]+\.[0-9A-Za-z]+$</c>).</param>
        /// <returns>
        /// 200 OK with the file bytes and the correct content type;
        /// 400 Bad Request if the filename is invalid;
        /// 404 Not Found if the file does not exist.
        /// </returns>
        [HttpGet()]
        [Route($"{nameof(Stream)}/{{{nameof(streamId)}}}/{{{nameof(filename)}}}")]
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Stream([FromRoute] string streamId, [FromRoute] string filename)
        {
            if (!Regex.IsMatch(filename, @"^[0-9A-Za-z_]+\.[0-9A-Za-z]+$"))
            {
                return this.BadRequest($"Filename \"{filename}\" is an invalid stream-file.");
            }
            string folder = Path.Combine(this._ApplicationConstants.GetDataFolder(), "CameraData", streamId, "Fragments");
            string path = Path.Combine(folder, filename);
            if (!System.IO.File.Exists(path))
            {
                return this.NotFound();
            }
            string contentType = filename.EndsWith(".m3u8") ? "application/vnd.apple.mpegurl" : "video/MP2T";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return this.File(fileBytes, contentType);
        }
    }
}