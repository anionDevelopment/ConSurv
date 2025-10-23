using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.Internals;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
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
        private readonly IStreamOrganizerService _StreamOrganizerService;
        private readonly IBusinessLogicService _CameraService;
        private readonly IRTSPManager _RTSPManager;
        private readonly IProcessManager _ProcessManager;
        private readonly IRuntimeData _RuntimeData;
        private readonly IApplicationConstants<Constants.CodeUnitSpecificConstants> _Constants;
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly CommandlineParameter _CommandlineParameter;
        private readonly IGRYLog _Log;
        private const ushort _LastUsedPortRangeBegin = 10_000;
        private ushort _LastUsedPort = _LastUsedPortRangeBegin;
        public CameraManagementService(IStreamOrganizerService streamOrganizerService, IBusinessLogicService businessLogicService, IGRYLog logger, CommandlineParameter commandlineParameter, IRTSPManager rTSPManager, IProcessManager processManager, IRuntimeData runtimeData, IInitializationService<CommandlineParameter> initializationService, IGRYLog log, IApplicationConstants<Constants.CodeUnitSpecificConstants> constants) : base(constants.ExecutionMode, logger)
        {
            this._StreamOrganizerService = streamOrganizerService;
            this._CameraService = businessLogicService;
            this._CommandlineParameter = commandlineParameter;
            this._RTSPManager = rTSPManager;
            this._ProcessManager = processManager;
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(2);
            this._InitializationService = initializationService;
            this._RuntimeData = runtimeData;
            this._Log = log;
            this._Constants = constants;
        }

        protected override void Run()
        {
            if (this._CommandlineParameter.RealRun && this._InitializationService.GetInitializationState() is Initialized)
            {
                foreach (Model.Base.Camera camera in this._CameraService.GetAllCameras().Values)
                {
                    try
                    {
                        this.ManageCamera(camera);
                    }
                    catch (Exception ex)
                    {
                        this._Log.Log($"Could not managee preview for camera {camera.Id}", ex);
                    }
                }
            }
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
            }
            CameraInternalsBase currentState = this.GetCurrentInternalState(camera);
            try
            {
                currentState.Accept(new EnsureDesiredConditionIsApplied(existingState));
            }
            catch (Exception ex)
            {
                NotAvailable newState = new NotAvailable(camera);
                this._RuntimeData.SetCameraInternals(newState);
                newState.Accept(new EnsureDesiredConditionIsApplied(currentState));
            }

            //camera.RecordMode.Accept(new ChangeRecordingModeVisitor(camera, this._RTSPManager));
        }
        private class EnsureDesiredConditionIsApplied : ICameraInternalsBaseVisitor
        {
            private readonly CameraInternalsBase _PreviousState;

            public EnsureDesiredConditionIsApplied(CameraInternalsBase previousState)
            {
                this._PreviousState = previousState;
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
                string overlay_folder = $"{this._Constants.GetDataFolder()}/Overlays/{camera.Id}";
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(overlay_folder);
                string overlay_file = $"{overlay_folder}\\overlay.png";
                this.CreateOverlayFile(camera, overlay_file);
                overlay_file = overlay_file.Replace("\\", "/");

                string path = $"Stream_{camera.Id}";

                //string fontfile = Path.Combine(location, "Fonts", "Noto", "NotoSans_Condensed-Regular.ttf");
                //GUtilities.AssertCondition(File.Exists(fontfile), "Font-file does not exist.");
                //TODO use "-reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_delay_max 10"
                string ffmpegArgument = "-fflags +genpts -rtsp_transport tcp -use_wallclock_as_timestamps 1  -i " + camera.VideoInformation.StreamURL + " -loop 1 -i " + overlay_file;
                //add the font if desired after "drawtext=":
                //fontfile='" + fontfile + "':
                ffmpegArgument = ffmpegArgument + " -filter_complex \"[0:v][1:v]overlay=0:0:format=auto,drawtext=fontsize=60:fontcolor=white:text='%{localtime\\:%Y-%m-%d %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.5:boxborderw=10:x=(w-text_w):y=(h-text_h)\"";//TODO consider timezone

                /*
                 
                 ffmpeg -rtsp_transport tcp \
  -i rtsp://user:pass@192.168.1.141/stream1 \
  -vf "drawtext=text='%{localtime\:%Y-%m-%d %H\\:%M\\:%S}':fontsize=24:fontcolor=white:x=10:y=10" \
  -c:v libx264 -preset ultrafast -tune zerolatency -c:a aac -fflags +genpts -use_wallclock_as_timestamps 1 \
  -f rtsp rtsp://127.0.0.1:8554/camera_6071FF \
  -vf fps=1/2 -update 1 preview_%03d.jpg

                 
                 */
                //TODO add black areas if defined
                ffmpegArgument = ffmpegArgument + " -c:v libx264 -c:a aac -preset ultrafast -avoid_negative_ts make_zero -vsync vfr -fflags nobuffer -f rtsp " + url;//ffmpeg takes the stream and redirects it to mediamtx
                ffmpegProcess = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument, null, null, $"Send stream of camera {camera.Id} to media-hub", $"StreamToMediaHubFrom-{camera.Id}", false);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess.ExitCode}.");

                Thread.Sleep(TimeSpan.FromSeconds(5));
                GRYLibrary.Core.Misc.Utilities.AssertCondition(IsRtspAvailable(url), $"Media-Hub for {camera.Id} is not available.");
                ffmpegProcessResult = ffmpegProcess;
                mediaMTXProcessResult = mediaMTXProcess;
                this._RuntimeData.SetCameraInternals(new Available(camera, ffmpegProcessResult, mediaMTXProcessResult, url));
                this._Log.Log($"Provided Camera {camera.Id} internally under \"{url}\".");

                //take screenshots
                string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "Screenshots", camera.Id);
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(screenshots_folder);
                string target_file = Path.Combine(screenshots_folder, "frame").Replace("\\", "/");
                string ffmpegArgument2 = $"-rtsp_transport tcp -i {url} -reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_delay_max 10 -vf fps=1/2 -qscale:v 2 {target_file}_%01d.jpg";
                ExternalProgramExecutor ffmpegProcess2 = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument2, null, null, $"Take screenshots of {camera.Id}", $"TakeScreenshotsOf-{camera.Id}", false);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(ffmpegProcess2.IsRunning, () => $"Process terminated unexpectedly with {ffmpegProcess2.ExitCode}.");

                Thread.Sleep(TimeSpan.FromSeconds(5));

                //temp:
                takescreenshots = ffmpegProcess2;
                mediamtx = mediaMTXProcess;
                streamtomediamtx = ffmpegProcess;

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

        private void CreateOverlayFile(Camera camera, string overlay_ffile)
        {
            int width = (int)camera.Overlay.Width;

            int height = (int)camera.Overlay.Height;

            var polygons = new List<List<SKPoint>>
            {
                //  new() { new SKPoint(100,100), new SKPoint(400,150), new SKPoint(350,400) },
                //  new() { new SKPoint(800,300), new SKPoint(1000,500), new SKPoint(700,550), new SKPoint(650,400) }
            };
            foreach (var polygon in camera.Overlay.Polygons)
            {
                polygons.Add(polygon.Points.Select(point => new SKPoint(point.X, point.Y)).ToList());
            }

            using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            foreach (var poly in polygons)
            {
                using var path = new SKPath();
                path.AddPoly(poly.ToArray(), close: true);
                canvas.DrawPath(path, paint);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = System.IO.File.OpenWrite(overlay_ffile);
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
