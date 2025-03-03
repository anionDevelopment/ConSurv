using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConSurvBackend.Core.Controller
{
    [ApiController]
    [Route(WebSocketController.ControllerRoute)]
    public class WebSocketController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(WebSocketController)}";

        private readonly string rtspUrl = "rtsp://192.168.1.141/stream1";
        private readonly IProcessManager _ProcessManager;
        public WebSocketController(IProcessManager processManager)
        {
            this._ProcessManager = processManager;
        }
        [HttpGet("videoStream")]
        public async Task GetVideoStream()
        {
            if (this.HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
                await this.HandleWebSocketAsync(webSocket);
            }
            else
            {
                this.HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            Process ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i {this.rtspUrl} -f image2pipe -vcodec mjpeg -acodec aac -ar 44100 -ac 1 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            this._ProcessManager.RegisterProcess(ffmpegProcess);
            ffmpegProcess.Start();

            byte[] buffer = new byte[4096];
            try
            {
                while (!ffmpegProcess.HasExited)
                {
                    int bytesRead = await ffmpegProcess.StandardOutput.BaseStream.ReadAsync(buffer);
                    if (bytesRead > 0)
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Senden der WebSocket-Daten: {ex.Message}");
            }
            finally
            {
                // Sicherstellen, dass der WebSocket geschlossen wird
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stream beendet", CancellationToken.None);
                }
            }
        }
    }
}
