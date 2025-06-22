using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
        [Route($"{nameof(StreamId)}/{{{nameof(cameraId)}}}")]
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult StreamId([FromRoute] string cameraId)
        {
            string streamId = _RTSPManager.StartStreamOfCamera(cameraId);
            string folder = Path.Combine(this._ApplicationConstants.GetDataFolder(), "Temp", "Streaming", streamId);
            GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(folder);
            return Ok(new JsonResult(streamId));
        }

        [HttpGet()]
        [Route($"{nameof(Stream)}/{{{nameof(streamId)}}}/{{{nameof(filename)}}}")]
        [Authenticate]
        [Authorize(CodeUnitSpecificConstants.RolenameUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public FileContentResult Stream([FromRoute] string streamId, [FromRoute] string filename)
        {
            //TODO check file to not contain any path-traversal-attempts
            string folder = Path.Combine(this._ApplicationConstants.GetDataFolder(), "Temp", "Streaming", Guid.NewGuid().ToString().Substring(0, 8));

            //----------------


            string filePath = Path.Combine(folder, filename);

            // FFmpeg-Prozess beim ersten Abruf starten
            //TODO start ffmpeg and ensure it will be exited automatically if the client is not loading files anymore
            /*
             if (_ffmpegProcess == null || _ffmpegProcess.HasExited)
             {
                 StartFfmpeg();
             }
            */

            // Warte ggf. auf erste Datei
            /*
            int waitAttempts = 0;
            while (!System.IO.File.Exists(filePath) && waitAttempts < 20)
            {
                await Task.Delay(200);
                waitAttempts++;
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Datei nicht vorhanden");
            }
            */

            string contentType = GetMimeType(filePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType);
        }
        /*
        private void StartFfmpeg()
        {
            Directory.CreateDirectory(OutputDir);
            string rtspUrl = "rtsp://dein-kamera-stream";

            string args = $"-i {rtspUrl} -c:v libx264 -c:a aac -f dash " +
                          "-seg_duration 2 -use_template 1 -use_timeline 1 " +
                          "-window_size 5 -extra_window_size 5 -remove_at_exit 1 " +
                          $"{OutputDir}/stream.mpd";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _ffmpegProcess = Process.Start(startInfo);
        }
        */
        private string GetMimeType(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".mpd" => "application/dash+xml",
                ".m4s" => "video/iso.segment",
                ".mp4" => "video/mp4",
                _ => "application/octet-stream",
            };
        }
    }
}