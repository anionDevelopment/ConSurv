using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private void StartFFmpegProcess(string filename)
        {
            string outputPath = Path.Combine(_HLSFolder, filename);

            string ffmpegArgs = $"-i {_rtspUrl} -c:v libx264 -preset ultrafast -tune zerolatency " +
                                "-c:a aac -b:a 128k -ac 2 " +
                                "-f hls -hls_time 2 -hls_list_size 5 -hls_flags delete_segments " +
                                $"{outputPath}.m3u8";

            Process ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = ffmpegArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            ffmpeg.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            ffmpeg.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

            ffmpeg.Start();
            ffmpeg.BeginOutputReadLine();
            ffmpeg.BeginErrorReadLine();

            Console.WriteLine("FFmpeg-Prozess gestartet...");
            ffmpeg.WaitForExit();
        }


        [HttpGet("x.m3u8")]
        public IActionResult GetHlsFile()
        {
            string filename = "cam01.m3u8";
            //TODO check if filename is valid (...to prevent every kind of path-traversal-attacks etc.)
            var filePath = Path.Combine(_HLSFolder, filename);

            if (!System.IO.File.Exists(filePath))
            {
                StartFFmpegProcess(filename); // FFmpeg nur starten, wenn die .m3u8-Datei noch nicht existiert
            }

            string contentType = filename.EndsWith(".m3u8") ? "application/vnd.apple.mpegurl" : "video/MP2T";
            return PhysicalFile(filePath, contentType);
        }
    }
}