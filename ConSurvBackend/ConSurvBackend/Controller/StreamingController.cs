using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ConSurvBackend.Core.Controller
{
    [ApiController]
    [Route(StreamingController.ControllerRoute)]
    public class StreamingController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(StreamingController)}";
        private readonly IApplicationConstants _ApplicationConstants;
        private readonly IRTSPManager _RTSPManager;
        private readonly IGRYLog _Log;
        public StreamingController(IApplicationConstants applicationConstants, IRTSPManager rtspManager, IGRYLog log)
        {
            this._ApplicationConstants = applicationConstants;
            this._RTSPManager = rtspManager;
            this._Log = log;
        }

        [HttpGet()]
        [Route($"{nameof(Stream)}/{{{nameof(streamId)}}}/{{{nameof(filename)}}}")]
        //TODO [Authenticate]
        //TODO [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Stream([FromRoute] string streamId, [FromRoute] string filename)
        {
            //TODO check filename to not contain any path-traversal-attempts
            string folder = Path.Combine(this._ApplicationConstants.GetDataFolder(), "Temp", "Streaming", streamId);
            string path = Path.Combine(folder, filename);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            string contentType = filename.EndsWith(".m3u8") ? "application/vnd.apple.mpegurl" : "video/MP2T";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, contentType);
        }
    }
}