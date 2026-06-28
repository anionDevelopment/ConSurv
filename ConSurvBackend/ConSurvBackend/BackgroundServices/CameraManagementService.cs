using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Misc.Logger;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.Internals;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class CameraManagementService : IteratingBackgroundService, ICameraManagementService
    {
        private readonly IBusinessLogicService _CameraService;
        private readonly IProcessManager _ProcessManager;
        private readonly IRuntimeData _RuntimeData;
        private readonly IApplicationConstants<Constants.CodeUnitSpecificConstants> _Constants;
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly CommandlineParameter _CommandlineParameter;
        private const ushort _LastUsedPortRangeBegin = 10_000;
        private ushort _LastUsedPort = _LastUsedPortRangeBegin;
        /// <summary>
        /// Initializes a new instance of <see cref="CameraManagementService"/> with all required dependencies.
        /// </summary>
        /// <param name="businessLogicService">Service used to retrieve camera data.</param>
        /// <param name="logger">Logger used by the base class.</param>
        /// <param name="commandlineParameter">Parsed command-line parameters for the current run.</param>
        /// <param name="processManager">Manager for spawning and tracking external processes.</param>
        /// <param name="runtimeData">Shared in-memory runtime state.</param>
        /// <param name="initializationService">Service that tracks the application initialization state.</param>
        /// <param name="log">Logger used for camera-management-specific log entries.</param>
        /// <param name="constants">Application-wide constants including data-folder paths.</param>
        /// <param name="codeUnitSpecificConfiguration">Persisted configuration containing video settings.</param>
        public CameraManagementService(IBusinessLogicService businessLogicService, ICameraManagementServiceLog logger, CommandlineParameter commandlineParameter, IProcessManager processManager, IRuntimeData runtimeData, IInitializationService<CommandlineParameter> initializationService, IApplicationConstants<Constants.CodeUnitSpecificConstants> constants, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration) : base(constants.ExecutionMode, logger.Logger)
        {
            this._CameraService = businessLogicService;
            this._CommandlineParameter = commandlineParameter;
            this._ProcessManager = processManager;
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(2);
            this._InitializationService = initializationService;
            this._RuntimeData = runtimeData;
            this._Constants = constants;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
        }

        /// <inheritdoc />
        protected override void Run()
        {
            this._Logger.Log($"ManageCameras", Microsoft.Extensions.Logging.LogLevel.Trace, false, true, true, true, true, () =>
            {
                if (this._InitializationService.GetInitializationState() is Initialized)
                {
                    ICollection<Camera> cameras = this._CameraService.GetAllCameras().Values;
                    this._Logger.Log("Cameras to manage: {" + string.Join(", ", cameras) + "}", Microsoft.Extensions.Logging.LogLevel.Debug);
                    foreach (Model.Base.Camera camera in cameras)
                    {
                        this._Logger.Log($"ManageCamera_{camera.Id}", Microsoft.Extensions.Logging.LogLevel.Debug, false, true, true, true, true, () => this.ManageCamera(camera));
                    }
                }
                else
                {
                    this._Logger.Log($"Wait until initialization is finished...", Microsoft.Extensions.Logging.LogLevel.Debug);
                }
            });
        }

        private void ManageCamera(Camera camera)
        {
            CameraInternalsBase? existingState;
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                if (!this._RuntimeData.GetCameraInternals().ContainsKey(camera.Id))
                {
                    this._RuntimeData.SetCameraInternals(new NotAvailable(camera));
                }
                existingState = this._RuntimeData.GetCameraInternals(camera.Id);
                CameraInternalsBase currentState = this.GetCurrentInternalState(camera);
                this._Logger.Log($"Camera {camera.Id}: tracked-state={existingState?.GetType().Name ?? "none"}, detected-state={currentState.GetType().Name}.", Microsoft.Extensions.Logging.LogLevel.Trace);
                try
                {
                    currentState.Accept(new EnsureDesiredConditionIsApplied(existingState, this));
                }
                catch (Exception e)
                {
                    this._Logger.Log($"Error while managing camera {camera.Id}. Resetting it to {nameof(NotAvailable)} and reapplying the desired condition.", e, Microsoft.Extensions.Logging.LogLevel.Debug);
                    NotAvailable newState = new NotAvailable(camera);
                    this._RuntimeData.SetCameraInternals(newState);
                    newState.Accept(new EnsureDesiredConditionIsApplied(currentState, this));
                }
            }

            //TODO camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RTSPManager));
        }

        private void LogDebug(string message)
        {
            this._Logger.Log(message, Microsoft.Extensions.Logging.LogLevel.Debug);
        }

        private void LogDebug(string message, Exception exception)
        {
            this._Logger.Log(message, exception, Microsoft.Extensions.Logging.LogLevel.Debug);
        }

        private class EnsureDesiredConditionIsApplied : ICameraInternalsBaseVisitor
        {
            private readonly CameraInternalsBase _PreviousState;
            private readonly CameraManagementService _CameraManagementService;

            public EnsureDesiredConditionIsApplied(CameraInternalsBase previousState, CameraManagementService cameraManagementService)
            {
                this._PreviousState = previousState;
                this._CameraManagementService = cameraManagementService;
            }

            public void Handle(Available available)
            {
                GRYLibrary.Core.Misc.Utilities.AssertCondition(available.MediaMTXProcess.IsRunning, $"MediaMTX terminated unexoectedly for {available.Camera.Id}.");
                GRYLibrary.Core.Misc.Utilities.AssertCondition(available.FFMPEGProcess.IsRunning, $"FFMPEG terminated unexoectedly for {available.Camera.Id}.");
                bool startProcesses = false;
                if (this._PreviousState.Camera.VideoInformation.StreamURL != available.Camera.VideoInformation.StreamURL)
                {
                    this._CameraManagementService.LogDebug($"Stream-URL of camera {available.Camera.Id} changed. Restarting its media-processes.");
                    if (this._PreviousState is Available prevous)
                    {
                        this.StopProcesses(prevous);
                    }
                    startProcesses = true;
                }
                if (this._PreviousState is NotAvailable)
                {
                    this._CameraManagementService.LogDebug($"Camera {available.Camera.Id} transitioned from {nameof(NotAvailable)} to {nameof(Available)}. Starting its media-processes.");
                    startProcesses = true;
                }
                if (startProcesses)
                {
                    this.StartProcesses(available);
                }
            }

            private void StartProcesses(Available available)
            {
                //  throw new NotImplementedException();
            }

            public void Handle(NotAvailable notAvailable)
            {
                if (this._PreviousState is Available previousAvailableState)
                {
                    this._CameraManagementService.LogDebug($"Camera {notAvailable.Camera.Id} is no longer available. Stopping its media-processes.");
                    this.StopProcesses(previousAvailableState);
                }
            }

            private void StopProcesses(Available available)
            {
                this._CameraManagementService.LogDebug($"Terminating media-processes of camera {available.Camera.Id}.");
                try
                {
                    if (available.MediaMTXProcess.IsRunning)
                    {
                        available.MediaMTXProcess.Terminate();
                    }
                }
                catch (Exception exception)
                {
                    this._CameraManagementService.LogDebug($"Error while terminating the MediaMTX-process of camera {available.Camera.Id}.", exception);
                }

                try
                {
                    if (available.FFMPEGProcess.IsRunning)
                    {
                        available.FFMPEGProcess.Terminate();
                    }

                }
                catch (Exception exception)
                {
                    this._CameraManagementService.LogDebug($"Error while terminating the FFMPEG-process of camera {available.Camera.Id}.", exception);
                }
            }
        }
        /// <summary>
        /// Determines the current internal availability state of a camera by checking whether its
        /// associated media processes (MediaMTX and FFMPEG) are running and the RTSP stream is reachable.
        /// </summary>
        /// <param name="camera">The camera whose internal state is evaluated.</param>
        /// <returns>
        /// An <see cref="Available"/> instance when all processes are running and the RTSP endpoint is
        /// reachable; otherwise a <see cref="NotAvailable"/> instance.
        /// </returns>
        private CameraInternalsBase GetCurrentInternalState(Camera camera)
        {
            if (this.TryGetMediaMTXProcess(camera, out ExternalProgramExecutor? ffmpegProcess, out ExternalProgramExecutor? mediaMTXProcess, out string? url))
            {
                return new Available(camera, ffmpegProcess!, mediaMTXProcess!, url!);
            }
            else
            {
                return new NotAvailable(camera);
            }
        }
        private static ExternalProgramExecutor mediamtx;
        private static ExternalProgramExecutor streamtomediamtx;
        private static ExternalProgramExecutor takescreenshots;
        private static ExternalProgramExecutor m3u8;
        private static ExternalProgramExecutor record;
        /// <summary>
        /// Tries to obtain (or create) the MediaMTX and FFMPEG processes that expose the camera's
        /// RTSP stream internally.  When no running processes exist yet the method starts MediaMTX,
        /// pipes the camera stream into it via FFMPEG, spawns a screenshot-capture process, an HLS
        /// segmenter process, and – if the camera is configured for continuous recording – a recording
        /// process.
        /// </summary>
        /// <param name="camera">The camera for which the media processes are required.</param>
        /// <param name="ffmpegProcessResult">
        /// When the method returns <see langword="true"/>, contains the running FFMPEG process;
        /// otherwise <see langword="null"/>.
        /// </param>
        /// <param name="mediaMTXProcessResult">
        /// When the method returns <see langword="true"/>, contains the running MediaMTX process;
        /// otherwise <see langword="null"/>.
        /// </param>
        /// <param name="url">
        /// When the method returns <see langword="true"/>, contains the internal RTSP URL under which
        /// the camera stream is available via MediaMTX; otherwise <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all required processes are running and the internal RTSP stream is
        /// reachable; <see langword="false"/> if any step failed.
        /// </returns>
        private bool TryGetMediaMTXProcess(Camera camera, out ExternalProgramExecutor? ffmpegProcessResult, out ExternalProgramExecutor? mediaMTXProcessResult, out string? url)
        {
            bool isAlreadyAvailable;
            CameraInternalsBase? cameraInternals;
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                isAlreadyAvailable = this._RuntimeData.GetCameraInternals().ContainsKey(camera.Id);
                if (isAlreadyAvailable)
                {
                    cameraInternals = this._RuntimeData.GetCameraInternals(camera.Id);
                }
                else
                {
                    cameraInternals = null;
                }
            }
            if (isAlreadyAvailable)
            {
                if (cameraInternals is Available available)
                {
                    ffmpegProcessResult = available.FFMPEGProcess;
                    mediaMTXProcessResult = available.MediaMTXProcess;
                    url = available.MediaMTXURL;
                    if (IsRtspAvailable(url, out string? errorMessage))
                    {
                        this._Logger.Log($"Camera {camera.Id} is still available internally under \"{url}\". Reusing the existing media-processes.", Microsoft.Extensions.Logging.LogLevel.Trace);
                        return true;
                    }
                    else
                    {
                        this._Logger.Log($"Camera {camera.Id} was available internally but its stream is no longer reachable. Recreating the media-processes. Details: {errorMessage}", Microsoft.Extensions.Logging.LogLevel.Debug);
                    }
                }
            }
            ExternalProgramExecutor ffmpegProcess;
            ExternalProgramExecutor mediaMTXProcess;
            try
            {
                this._Logger.Log($"(Re)starting media-processes for camera {camera.Id} (stream-URL: \"{Misc.Utilities.EscapeBasicAuthPasswords(camera.VideoInformation.StreamURL)}\").", Microsoft.Extensions.Logging.LogLevel.Debug);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(IsRtspAvailable(camera.VideoInformation.StreamURL, out string? errorMessage), $"Camera {camera.Id} is not available: " + errorMessage);
                Thread.Sleep(TimeSpan.FromSeconds(2));//wait until the camera is available again after probing it
                string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                //mediamtx
                string mediaMTXFolder = Path.Combine(location, "MediaMTX");
                string mediaMTXExecutable = Path.Combine(mediaMTXFolder, "mediamtx");
                if (GRYLibrary.Core.OperatingSystem.OperatingSystem.GetCurrentOperatingSystem() is GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems.Windows)
                {
                    mediaMTXExecutable = mediaMTXExecutable + ".exe";
                }
                ushort mediaMTXPort = this.GetNewFreePort();
                this._Logger.Log($"Using port {mediaMTXPort} for the media-hub of camera {camera.Id}.", Microsoft.Extensions.Logging.LogLevel.Debug);
                string configurationFileContent = @$"rtspAddress: ""0.0.0.0:{mediaMTXPort}""
rtmp: no
hls: no
webrtc: no
srt: no
protocols: [tcp]
paths:
  camera_{camera.Id}:
    overridePublisher: yes
".Replace("\r", string.Empty);
                string configFileName = $"MediaMTXCameraConfiguration_{camera.Id}.yml";
                string configFile = Path.Combine(mediaMTXFolder, configFileName);
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(configFile);
                File.WriteAllText(configFile, configurationFileContent);
                this._Logger.Log($"Content of {configFile}:\n\"" + File.ReadAllText(configFile, new UTF8Encoding(false)) + "\"", Microsoft.Extensions.Logging.LogLevel.Trace);
                //FIXME: mediamtx will not be exited after terminating consurvbackend, even if it should be (by espoc)
                mediaMTXProcess = this._ProcessManager.GetBackgroundProcess(mediaMTXExecutable, configFileName, mediaMTXFolder, null, $"Media-hub for {camera.Id}", $"MediaHubFor{camera.Id}", false);
                url = $"rtsp://127.0.0.1:{mediaMTXPort}/camera_{camera.Id}";
                Thread.Sleep(TimeSpan.FromSeconds(2));
                GRYLibrary.Core.Misc.Utilities.AssertCondition(mediaMTXProcess.IsRunning, () => $"Process terminated unexpectedly with {mediaMTXProcess.ExitCode}.");
                Thread.Sleep(TimeSpan.FromSeconds(3));

                //stream to media hub
                string overlay_folder = $"{this._Constants.GetDataFolder()}/CameraData/{camera.Id}/Overlays";
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(overlay_folder);
                string overlay_file = $"{overlay_folder}\\overlay.png";
                overlay_file = overlay_file.Replace("\\", "/");
                this.CreateOverlayFile(camera, overlay_file);
                string path = $"Stream_{camera.Id}";
                string ffmpegArgument = "-fflags +genpts -rtsp_transport tcp -use_wallclock_as_timestamps 1  -i " + camera.VideoInformation.StreamURL + " -loop 1 -i " + overlay_file;
                ffmpegArgument = ffmpegArgument + " -filter_complex \"[0:v][1:v]overlay=0:0:format=auto,drawtext=fontsize=60:fontcolor=white:text='" + camera.Name + " (" + camera.Id + ") %{localtime\\:%Y-%m-%d %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.5:boxborderw=10:x=(w-text_w):y=(h-text_h)\"";//TODO consider camera-timezone in timestamp
                ffmpegArgument = ffmpegArgument + " -c:v libx264 -c:a aac -preset ultrafast -tune zerolatency -g 50 -keyint_min 50 -sc_threshold 0 -avoid_negative_ts make_zero -vsync vfr -fflags nobuffer -metadata title=\"Camera-" + camera.Id + "\" -f rtsp " + url;//ffmpeg takes the stream and redirects it to mediamtx
                string purpose = $"StreamToMediaHubFrom-{camera.Id}";
                ffmpegProcess = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument, null, null, $"Send stream of camera {camera.Id} to media-hub", purpose, false);
                Thread.Sleep(TimeSpan.FromSeconds(5));
                GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess.IsRunning, () => $"Process \"{purpose}\" terminated unexpectedly with {ffmpegProcess.ExitCode}.");

                //assert stream is available
                GRYLibrary.Core.Misc.Utilities.AssertCondition(IsRtspAvailable(url, out string? message), () =>
                {
                    string result = $"{message}\nMedia-Hub for {camera.Id} is not available.";
                    try
                    {
                        string[] stdOut;
                        string[] stdErr;
                        if (mediaMTXProcess.IsRunning)
                        {
                            stdOut = mediaMTXProcess.AllStdOutLinesPartially;
                            stdErr = mediaMTXProcess.AllStdErrLinesPartially;
                        }
                        else
                        {
                            result = result + "\nExit-code: " + mediaMTXProcess.ExitCode.ToString();
                            stdOut = mediaMTXProcess.AllStdOutLines;
                            stdErr = mediaMTXProcess.AllStdErrLines;
                        }
                        result = result + "\nStdOut: {\n" + string.Join("\n", stdOut) + "\n};\nStdErr: {\n" + string.Join("\n", stdErr) + "\n}";
                    }
                    catch
                    {
                        GRYLibrary.Core.Misc.Utilities.NoOperation();
                    }
                    return result;
                });
                ffmpegProcessResult = ffmpegProcess;
                mediaMTXProcessResult = mediaMTXProcess;
                this._RuntimeData.SetCameraInternals(new Available(camera, ffmpegProcessResult, mediaMTXProcessResult, url));
                this._Logger.Log($"Provided Camera {camera.Id} internally under \"{url}\".");

                //take screenshots (means: previews)
                string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(screenshots_folder);
                string target_file = Path.Combine(screenshots_folder, "frame").Replace("\\", "/");
                string ffmpegArgument2 = $"-rtsp_transport tcp -i {url} -reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_delay_max 10 -vf fps=1/2 -qscale:v 2 -strftime 1 {target_file}_%Y-%m-%dT%H-%M-%S.jpg";
                ExternalProgramExecutor ffmpegProcess2 = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument2, null, null, $"Take screenshots of {camera.Id}", $"TakeScreenshotsOf-{camera.Id}", false);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess2.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess2.ExitCode}.");

                //prepare m3u8 stream
                string fradments_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Fragments");
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(fradments_folder);
                fradments_folder = fradments_folder.Replace("\\", "/");
                uint timeOfFragmentInSeconds = 2;
                uint amountOfFragments = 1;
                string ffmpegArgument3 = $"-rtsp_transport tcp -i {url} -c:v copy -c:a aac -f hls -hls_time {timeOfFragmentInSeconds} -hls_list_size {amountOfFragments} -hls_flags delete_segments -hls_segment_filename {fradments_folder}/segment_%01d.ts {fradments_folder}/stream.m3u8";
                ExternalProgramExecutor ffmpegProcess3 = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument3, null, null, $"Provide m3u8-stream {camera.Id}", $"ProvideM3U8Stream-{camera.Id}", false);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess3.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess3.ExitCode}.");
                m3u8 = ffmpegProcess3;

                if (camera.RecordMode is RecordAlways)
                {
                    //record
                    this._Logger.Log($"Start recording {camera.Id}.");
                    string target_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Recordings");
                    GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(target_folder);
                    target_folder = target_folder.Replace("\\", "/");
                    uint videoLengthInSeconds = (uint)Math.Round(this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength.TotalSeconds);
                    string ffmpegArgument4 = $"-rtsp_transport tcp -i {url} -c copy -f segment -strftime 1 -segment_time {videoLengthInSeconds} -reset_timestamps 1 {target_folder}/Camera_{camera.Id}_%Y-%m-%d-%H-%M-%S.mp4";
                    ExternalProgramExecutor ffmpegProcess4 = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument4, null, null, $"Record camera-stream {camera.Id}", $"RecordCameraStream-{camera.Id}", false);
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess4.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess4.ExitCode}.");
                    record = ffmpegProcess4;
                }

                takescreenshots = ffmpegProcess2;
                mediamtx = mediaMTXProcess;
                streamtomediamtx = ffmpegProcess;

                Thread.Sleep(TimeSpan.FromSeconds(1));

                return true;

            }
            catch (Exception e)
            {
                this._Logger.Log($"Could not start media-processes for {camera.Id}", e);
                ffmpegProcessResult = null;
                mediaMTXProcessResult = null;
                url = null;
                this._RuntimeData.SetCameraInternals(new NotAvailable(camera));
                return false;
            }
        }

        /// <summary>
        /// Renders the camera's overlay polygon configuration into a PNG file that can be composited
        /// on top of the live video stream by FFMPEG.
        /// </summary>
        /// <param name="camera">The camera whose overlay configuration is used.</param>
        /// <param name="overlayFile">Absolute path of the PNG file to write.</param>
        private void CreateOverlayFile(Camera camera, string overlayFile)
        {
            int width = (int)camera.Overlay.Width;

            int height = (int)camera.Overlay.Height;

            List<List<SKPoint>> polygons = new List<List<SKPoint>>();
            foreach (Polygon polygon in camera.Overlay.Polygons)
            {
                polygons.Add(polygon.Points.Select(point => new SKPoint(point.X, point.Y)).ToList());
            }

            using SKSurface surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            using SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            foreach (List<SKPoint> poly in polygons)
            {
                using SKPath path = new SKPath();
                path.AddPoly(poly.ToArray(), close: true);
                canvas.DrawPath(path, paint);
            }

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream stream = System.IO.File.OpenWrite(overlayFile);
            data.SaveTo(stream);
            this._Logger.Log($"Created overlay-file for camera {camera.Id} ({width}x{height}, {polygons.Count} polygon(s)) at \"{overlayFile}\".", Microsoft.Extensions.Logging.LogLevel.Debug);
        }

        /// <summary>
        /// Returns the next port number in the sequentially allocated port range starting at
        /// <see cref="_LastUsedPortRangeBegin"/>. Wraps around to the range begin when
        /// <see cref="ushort.MaxValue"/> is reached.
        /// </summary>
        /// <returns>A port number that has not been handed out since the last wrap-around.</returns>
        private ushort GetNewFreePort()
        {
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                if (this._LastUsedPort == ushort.MaxValue)
                {
                    this._LastUsedPort = _LastUsedPortRangeBegin;
                }
                this._LastUsedPort = (ushort)(this._LastUsedPort + 1);
                return this._LastUsedPort;
            }
        }

        /// <summary>
        /// Probes the given RTSP URL with <c>ffprobe</c> to determine whether the stream is currently
        /// accessible.
        /// </summary>
        /// <param name="rtspUrl">The RTSP URL to probe.</param>
        /// <returns>
        /// <see langword="true"/> if <c>ffprobe</c> exits with code 0; <see langword="false"/> if the
        /// URL is unreachable or probing fails for any reason.
        /// </returns>
        private static bool IsRtspAvailable(string rtspUrl, out string? message)
        {
            try
            {
                ExternalProgramExecutor e = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
                {
                    Program = "ffprobe",
                    Argument = $"-v error -i \"{rtspUrl}\"",
                    TimeoutInMilliseconds = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                    WaitingState = new RunSynchronously(),
                    Verbosity = Verbosity.Quiet,
                });
                e.Configuration.WaitingState = new RunSynchronously()
                {
                    ThrowErrorIfExitCodeIsNotZero = false,
                };
                e.Run();
                bool success = e.ExitCode == 0;
                if (success)
                {
                    message = null;
                }
                else
                {
                    message = $"Failed to probe RTSP URL \"{rtspUrl}\". StdOut: {String.Join('\n', e.AllStdOutLines)}; StdErr: {String.Join('\n', e.AllStdErrLines)}";
                }
                return success;
            }
            catch (Exception ex)
            {
                message = GRYLibrary.Core.Misc.Utilities.GetExceptionMessage(ex);
                return false;
            }
        }
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //add dispose logic here if required
            }
            base.Dispose(disposing);
        }
    }
}
