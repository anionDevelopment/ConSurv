using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConSurvBackend.Core.Controller
{

    [Route("ws2")]
    public class WebSocket2Controller : ControllerBase//ConnectionHandler
    {
        //private const string FFmpegCommand = "-rtsp_transport tcp -crf 23 -i \"rtsp://192.168.1.141/stream1\" -c:v libx264 -preset veryfast -crf 23 -c:a aac -hls_time 2 -hls_list_size 5 -f hls -";//TODO consider using  -hwaccel
        // private const string FFmpegCommand = "-i \"rtsp://192.168.1.141/stream1\" -c:v libx264 -preset veryfast -crf 23 -c:a aac -f hls -";
        //private const string FFmpegCommand = "-rtsp_transport tcp -fflags nobuffer -i \"rtsp://192.168.1.141/stream1\" -f mpegts -b:v 1000k -bf 0 pipe:1";
        //private const string FFmpegCommand = "-rtsp_transport tcp -fflags nobuffer -i \"rtsp://192.168.1.141/stream1\" -f mpegts -c:v h264 -an -codec:a aac -strict -2 -b:v 1000k -bf 0 pipe:1";
        //private const string FFmpegCommand = "-rtsp_transport tcp -fflags nobuffer -i \"rtsp://192.168.1.141/stream1\" -f mpegts -b:v 1000k -bf 0 pipe:1";//sending
        //private const string FFmpegCommand = "-rtsp_transport tcp -fflags nobuffer -i \"rtsp://192.168.1.141/stream1\" -c:v libx264 -preset ultrafast -tune zerolatency -profile:v high -level 4.2 -pix_fmt yuv420p -c:a aac -b:a 128k -f mpegts pipe:1";
        private const string FFmpegCommand = "-rtsp_transport tcp -i rtsp://192.168.1.141/stream1 -c:v libx264 -preset ultrafast -tune zerolatency -profile:v high -level 4.2 -pix_fmt yuv420p -c:a aac -b:a 128k -movflags frag_keyframe+empty_moov -f mp4 pipe:1";

        private readonly IGeneralLogger _Logger;
        public WebSocket2Controller(IGeneralLogger logger)
        {
            this._Logger = logger;
        }
        [HttpGet("wsStream")]
        public async Task Get()
        {
            if (this.HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
                await this.StreamRtspToWebSocket(webSocket);
            }
            else
            {
                this.HttpContext.Response.StatusCode = 400;
            }
        }
        /*
        public override Task OnConnectedAsync(ConnectionContext connection)
        {
            throw new NotImplementedException();
        }
        */
        private async Task StreamRtspToWebSocket(WebSocket webSocket)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = FFmpegCommand,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            //using var ffmpegOutput = process.StandardOutput.BaseStream;
            using var ffmpegOutput = process.StandardError.BaseStream;
            this._Logger.Log("Start...", Microsoft.Extensions.Logging.LogLevel.Debug);
            byte[] buffer = new byte[4096];
            while (webSocket.State == WebSocketState.Open)
            {
                int bytesRead = await ffmpegOutput.ReadAsync(buffer);
                if (bytesRead > 0)
                {
                    //this._Logger.Log($"read {bytesRead} bytes...", Microsoft.Extensions.Logging.LogLevel.Debug);
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
            this._Logger.Log("Finished.", Microsoft.Extensions.Logging.LogLevel.Debug);

            process.Kill();
            webSocket.Dispose();
        }
    }
}
