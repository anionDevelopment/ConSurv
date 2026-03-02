using ConSurvBackend.Core.Configuration;
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
using GRYLibrary.Core.Logging.GRYLogger;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private readonly IGRYLog _Log;
        private const ushort _LastUsedPortRangeBegin = 10_000;
        private ushort _LastUsedPort = _LastUsedPortRangeBegin;
        public CameraManagementService(IBusinessLogicService businessLogicService, IGRYLog logger, CommandlineParameter commandlineParameter, IProcessManager processManager, IRuntimeData runtimeData, IInitializationService<CommandlineParameter> initializationService, IGRYLog log, IApplicationConstants<Constants.CodeUnitSpecificConstants> constants, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration) : base(constants.ExecutionMode, logger)
        {
            this._CameraService = businessLogicService;
            this._CommandlineParameter = commandlineParameter;
            this._ProcessManager = processManager;
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(2);
            this._InitializationService = initializationService;
            this._RuntimeData = runtimeData;
            this._Log = log;
            this._Constants = constants;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
        }

        protected override void Run()
        {
            this._Log.Log($"ManageCameras", Microsoft.Extensions.Logging.LogLevel.Trace, false, true, true, true, true, () =>
            {
                if (this._InitializationService.GetInitializationState() is Initialized)
                {
                    ICollection<Camera> cameras = this._CameraService.GetAllCameras().Values;
                    this._Log.Log("Cameras to manage: {" + string.Join(", ", cameras) + "}", Microsoft.Extensions.Logging.LogLevel.Debug);
                    foreach (Model.Base.Camera camera in cameras)
                    {
                        this._Log.Log($"ManageCamera_{camera.Id}", Microsoft.Extensions.Logging.LogLevel.Debug, false, true, true, true, true, () => this.ManageCamera(camera));
                    }
                }
                else
                {
                    this._Log.Log($"Wait until initialization is finished...", Microsoft.Extensions.Logging.LogLevel.Debug);
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
                try
                {
                    currentState.Accept(new EnsureDesiredConditionIsApplied(existingState, this));
                }
                catch (Exception e)
                {
                    this._Log.Log($"Error while managing camera {camera.Id}.", e, Microsoft.Extensions.Logging.LogLevel.Debug);
                    NotAvailable newState = new NotAvailable(camera);
                    this._RuntimeData.SetCameraInternals(newState);
                    newState.Accept(new EnsureDesiredConditionIsApplied(currentState, this));
                }
            }

            //TODO camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RTSPManager));
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
                    if (this._PreviousState is Available prevous)
                    {
                        this.StopProcesses(prevous);
                    }
                    startProcesses = true;
                }
                if (this._PreviousState is NotAvailable)
                {
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
                    this.StopProcesses(previousAvailableState);
                }
            }

            private void StopProcesses(Available available)
            {
                try
                {
                    if (available.MediaMTXProcess.IsRunning)
                    {
                        available.MediaMTXProcess.Terminate();
                    }
                }
                catch
                {
                    //TODO log exception
                }

                try
                {
                    if (available.FFMPEGProcess.IsRunning)
                    {
                        available.FFMPEGProcess.Terminate();
                    }

                }
                catch
                {
                    //TODO log exception
                }
            }
        }
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
                    if (IsRtspAvailable(url))
                    {
                        return true;
                    }
                }
            }
            ExternalProgramExecutor ffmpegProcess;
            ExternalProgramExecutor mediaMTXProcess;
            try
            {
                GRYLibrary.Core.Misc.Utilities.AssertCondition(IsRtspAvailable(camera.VideoInformation.StreamURL), $"Camera {camera.Id} is not available.");
                string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                //mediamtx
                string mediaMTXFolder = Path.Combine(location, "MediaMTX");
                string mediaMTXExecutable = Path.Combine(mediaMTXFolder, "mediamtx");
                if (GRYLibrary.Core.OperatingSystem.OperatingSystem.GetCurrentOperatingSystem() is GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems.Windows)
                {
                    mediaMTXExecutable = mediaMTXExecutable + ".exe";
                }
                ushort mediaMTXPort = this.GetNewFreePort();
                string configurationFileContent = @$"rtspAddress: ""0.0.0.0:{mediaMTXPort}""
paths:
  camera_{camera.Id}:
    sourceOnDemand: no
    runOnReadRestart: no
".Replace("\r", string.Empty);
                string configFileName = $"MediaMTXCameraConfiguration_{camera.Id}.yml";
                string configFile = Path.Combine(mediaMTXFolder, configFileName);
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(configFile);
                File.WriteAllText(configFile, configurationFileContent);
                mediaMTXProcess = this._ProcessManager.GetBackgroundProcess(mediaMTXExecutable, configFileName, mediaMTXFolder, null, $"Media-hub for {camera.Id}", $"MediaHubFor{camera.Id}", false);
                url = $"rtsp://127.0.0.1:{mediaMTXPort}/camera_{camera.Id}";
                Thread.Sleep(TimeSpan.FromSeconds(3));
                GRYLibrary.Core.Misc.Utilities.AssertCondition(mediaMTXProcess.IsRunning, () => $"Process terminated unexpectedly with {mediaMTXProcess.ExitCode}.");

                //stream to media hub
                ushort ffmpegPort = this.GetNewFreePort();
                string overlay_folder = $"{this._Constants.GetDataFolder()}/CameraData/{camera.Id}/Overlays";
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(overlay_folder);
                string overlay_file = $"{overlay_folder}\\overlay.png";
                overlay_file = overlay_file.Replace("\\", "/");
                this.CreateOverlayFile(camera, overlay_file);

                string path = $"Stream_{camera.Id}";

                string ffmpegArgument = "-fflags +genpts -rtsp_transport tcp -use_wallclock_as_timestamps 1  -i " + camera.VideoInformation.StreamURL + " -loop 1 -i " + overlay_file;
                ffmpegArgument = ffmpegArgument + " -filter_complex \"[0:v][1:v]overlay=0:0:format=auto,drawtext=fontsize=60:fontcolor=white:text='" + camera.Name + " (" + camera.Id + ") %{localtime\\:%Y-%m-%d %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.5:boxborderw=10:x=(w-text_w):y=(h-text_h)\"";//TODO consider camera-timezone in timestamp

                ffmpegArgument = ffmpegArgument + " -c:v libx264 -c:a aac -preset ultrafast -tune zerolatency -g 50 -keyint_min 50 -sc_threshold 0 -avoid_negative_ts make_zero -vsync vfr -fflags nobuffer -metadata title=\"Camera-" + camera.Id + "\" -f rtsp " + url;//ffmpeg takes the stream and redirects it to mediamtx
                ffmpegProcess = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument, null, null, $"Send stream of camera {camera.Id} to media-hub", $"StreamToMediaHubFrom-{camera.Id}", false);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess.ExitCode}.");

                Thread.Sleep(TimeSpan.FromSeconds(5));
                GRYLibrary.Core.Misc.Utilities.AssertCondition(IsRtspAvailable(url), $"Media-Hub for {camera.Id} is not available.");
                ffmpegProcessResult = ffmpegProcess;
                mediaMTXProcessResult = mediaMTXProcess;
                this._RuntimeData.SetCameraInternals(new Available(camera, ffmpegProcessResult, mediaMTXProcessResult, url));
                this._Log.Log($"Provided Camera {camera.Id} internally under \"{url}\".");

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
                    string target_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Recordings");
                    GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(target_folder);
                    target_folder = target_folder.Replace("\\", "/");
                    uint videoLengthInSeconds = (uint)Math.Round(this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength.TotalSeconds);
                    string ffmpegArgument4 = $"-rtsp_transport tcp -i {url} -c copy -f segment -strftime 1 -segment_time {videoLengthInSeconds} -reset_timestamps 1 {target_folder}/Camera_{camera.Id}_%Y-%m-%d-%H-%M-%S.mp4";
                    ExternalProgramExecutor ffmpegProcess4 = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument4, null, null, $"Record camera-stream {camera.Id}", $"RecordCameraStream-{camera.Id}", false);
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess4.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess4.ExitCode}.");
                    record = ffmpegProcess4;
                }
                else if (camera.RecordMode is RecordOnMovements recordOnMovements)
                {
                    //TODO run ffmpeg with something like "-i rtsp://camera/stream -vf "select=gt(scene\,0.1)" -vsync vfr -f null -" async and start recording on events for motion-detection (0.1 is the threshold which mus be taken from recordOnMovements)

                }

                takescreenshots = ffmpegProcess2;
                mediamtx = mediaMTXProcess;
                streamtomediamtx = ffmpegProcess;

                Thread.Sleep(TimeSpan.FromSeconds(5));

                return true;

            }
            catch (Exception e)
            {
                this._Log.Log($"Could not start Media-hub for {camera.Id}", e);
                ffmpegProcessResult = null;
                mediaMTXProcessResult = null;
                url = null;
                this._RuntimeData.SetCameraInternals(new NotAvailable(camera));
                return false;
            }
        }

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
        }

        private ushort GetNewFreePort()
        {
            if (this._LastUsedPort == ushort.MaxValue)
            {
                this._LastUsedPort = _LastUsedPortRangeBegin;
            }
            this._LastUsedPort = (ushort)(this._LastUsedPort + 1);

            return this._LastUsedPort;
        }

        private static bool IsRtspAvailable(string rtspUrl)
        {
            try
            {
                ExternalProgramExecutor e = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
                {
                    Program = "ffprobe",
                    Argument = $"-v error -rtsp_transport tcp -i \"{rtspUrl}\"",
                    TimeoutInMilliseconds = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                    WaitingState = new RunSynchronously(),
                    Verbosity = Verbosity.Quiet
                });
                e.Run();
                return e.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
