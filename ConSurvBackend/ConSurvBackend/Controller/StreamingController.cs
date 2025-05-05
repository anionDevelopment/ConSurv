using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ConSurvBackend.Core.Controller
{
    [Route("hls")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        //private readonly string _HLSFolder = Path.Combine(Directory.GetCurrentDirectory(), "hls"); // HLS-Speicherort
        private readonly string _HLSFolder = @"C:\Temp";

        // Methode, um den FFmpeg-Prozess zu starten

        private readonly IApplicationConstants _ApplicationConstants;
        private readonly IGRYLog _Log;
        public StreamingController(IApplicationConstants applicationConstants, IGRYLog log)
        {
            _ApplicationConstants = applicationConstants;
            _Log = log;
        }


        [HttpGet("x.m3u8")]
        public IActionResult GetHlsFile()
        {
            string filename = "cam01.m3u8";
            //TODO check if filename is valid (...to prevent every kind of path-traversal-attacks etc.)
            var filePath = Path.Combine(this._HLSFolder, filename);

            //TODO read from disk

            string contentType = filename.EndsWith(".m3u8") ? "application/vnd.apple.mpegurl" : "video/MP2T";
            return this.PhysicalFile(filePath, contentType);
        }
    }
}