using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConSurvBackend.Core.Controller
{

    [Route("ws2")]
    public class WebSocket2Controller : ControllerBase//ConnectionHandler
    {
        private const string FFmpegCommand = "-rtsp_transport tcp -crf 23 -i \"rtsp://tpuser:GgrechuH_fzing655f@192.168.1.141/stream1\" -c:v libx264 -preset veryfast -crf 23 -c:a aac -hls_time 2 -hls_list_size 5 -f hls -";//TODO consider using  -hwaccel

        [HttpGet("wsStream")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await StreamRtspToWebSocket(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
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
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            byte[] buffer = new byte[4096];

            using var stream = process.StandardOutput.BaseStream;
            while (!webSocket.CloseStatus.HasValue)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }

            process.Kill();
            webSocket.Dispose();
        }
    }
}
