using ConSurvBackend.Core.Constants;
using GRYLibrary.Core.APIServer.Settings.Configuration;
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
    [ApiController]
    [Route(WebSocketController.ControllerRoute)]
    public class WebSocketController : ControllerBase
    {
        public const string ControllerRoute = $"{ServerConfiguration.APIRoutePrefix}/v{GeneralConstants.CodeUnitMajorVersion}/{nameof(WebSocketController)}";

        private readonly string rtspUrl = "rtsp://tpuser:GgrechuH_fzing655f@192.168.1.141/stream1";  // Deine RTSP-URL

        [HttpGet("videoStream")]
        public async Task GetVideoStream()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400; // Bad Request
            }
        }

        private async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            // Start FFmpeg als Subprozess und leite seine Ausgabe zu uns weiter
            Process ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i {rtspUrl} -f image2pipe -vcodec mjpeg -acodec aac -ar 44100 -ac 1 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            ffmpegProcess.Start();

            byte[] buffer = new byte[4096]; // Buffer für Video- und Audiodaten

            // WebSocket-Nachricht senden, solange der FFmpeg-Prozess Daten liefert
            try
            {
                while (true)
                {
                    int bytesRead = await ffmpegProcess.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        // Sende die Daten über WebSocket
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                    else
                    {
                        // Wenn FFmpeg keine Daten mehr liefert, beenden wir den Stream
                        break;
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
