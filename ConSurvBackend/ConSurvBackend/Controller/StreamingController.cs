using ConSurvBackend.Core.Miscellaneous;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;

namespace ConSurvBackend.Core.Controller
{
    [Route("hls")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        //private readonly string _HLSFolder = Path.Combine(Directory.GetCurrentDirectory(), "hls"); // HLS-Speicherort
        private readonly string _HLSFolder = @"C:\Temp";
        private readonly string _rtspUrl = "rtsp://192.168.1.141/stream1";//ffmpeg -i rtsp://192.168.1.141/stream1 -c:v libx264 -preset ultrafast -tune zerolatency -c:a aac -b:a 128k -ac 2 -f hls -hls_time 2 -hls_list_size 5 -hls_flags delete_segments -hls_allow_cache 0 x.m3u8

        // Methode, um den FFmpeg-Prozess zu starten

        private readonly IApplicationConstants _ApplicationConstants;
        private readonly IGRYLog _Log;
        public StreamingController(IApplicationConstants applicationConstants, IGRYLog log)
        {
            _ApplicationConstants = applicationConstants;
            _Log = log;
        }
        private void StartFFmpegProcess(string filename)
        {
            string outputPath = Path.Combine(this._HLSFolder, filename);
            string ffmpegArgs = $"-i {this._rtspUrl} -c:v libx264 -preset ultrafast -tune zerolatency " +
                                "-c:a aac -b:a 128k -ac 2 " +
                                "-f hls -hls_time 2 -hls_list_size 5 -hls_flags delete_segments " +
                                $"{outputPath}.m3u8";
            Process ffmpeg = Utilities.GetBackgroundProcess("ffmpeg", ffmpegArgs, null, _ApplicationConstants.GetConfigurationFolder(), p => { },_Log,"Client wants to stream");
            ffmpeg.BeginOutputReadLine();
            ffmpeg.BeginErrorReadLine();
            ffmpeg.WaitForExit();
        }


        [HttpGet("x.m3u8")]
        public IActionResult GetHlsFile()
        {
            string filename = "cam01.m3u8";
            //TODO check if filename is valid (...to prevent every kind of path-traversal-attacks etc.)
            var filePath = Path.Combine(this._HLSFolder, filename);

            if (!System.IO.File.Exists(filePath))
            {
                this.StartFFmpegProcess(filename); // FFmpeg nur starten, wenn die .m3u8-Datei noch nicht existiert
            }

            string contentType = filename.EndsWith(".m3u8") ? "application/vnd.apple.mpegurl" : "video/MP2T";
            return this.PhysicalFile(filePath, contentType);
        }
    }
}