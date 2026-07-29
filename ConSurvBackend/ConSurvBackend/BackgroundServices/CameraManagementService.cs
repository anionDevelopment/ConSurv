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
using System.Net;
using System.Net.Sockets;
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
        private readonly string _EntryAssemblyLocation;
        private const ushort _LastUsedPortRangeBegin = 20_000;
        private const ushort _LastUsedPortRangeEnd = 30_000;
        /// <summary>
        /// The port handed out by the most recent successful call to <see cref="GetNewFreePort"/>, or
        /// <see langword="null"/> if no port has been allocated yet. <see langword="null"/> makes the very
        /// first allocated candidate <see cref="_LastUsedPortRangeBegin"/> instead of begin+1.
        /// </summary>
        private ushort? _LastUsedPort = null;
        /// <summary>
        /// Holds all currently relevant runtime information (processes, port, URL) per camera, keyed by
        /// camera-id. This is the single source of truth for which processes belong to a camera and is
        /// used both for diagnostics and to be able to terminate every process a camera owns.
        /// </summary>
        private readonly IDictionary<string/*camera-id*/, CameraRuntimeInformation> _CameraRuntimeInformation = new Dictionary<string, CameraRuntimeInformation>();
        /// <summary>
        /// Caches whether FFmpeg can use NVIDIA-GPU-acceleration (NVENC) on this machine. <see langword="null"/>
        /// until it has been detected once (lazily, on the first camera-start) via
        /// <see cref="GpuAccelerationIsAvailable"/>; afterwards the detected value is reused for the whole
        /// process-lifetime so the (comparatively expensive) probe runs only a single time.
        /// </summary>
        private bool? _GpuAccelerationAvailable = null;
        private readonly object _GpuDetectionLock = new();
        /// <summary>
        /// Ids of cameras whose stream-process failed to start with GPU-acceleration and which are therefore
        /// permanently (for the rest of the process-lifetime) handled on the CPU. NVENC can be available in
        /// general while still refusing an additional encoding-session, because consumer-GPUs limit the amount
        /// of concurrent NVENC-sessions. Without this the affected camera would fail to start over and over
        /// again in every reconciliation-iteration instead of falling back to the CPU-encoder.
        /// </summary>
        private readonly ISet<string/*camera-id*/> _CamerasWithDisabledGpuAcceleration = new HashSet<string>();
        /// <summary>
        /// Encoder-arguments used when the video is encoded by the GPU. <c>-rc vbr -cq 23 -b:v 0</c> selects a
        /// quality-based rate-control comparable to the CRF-default of <c>libx264</c>; without it NVENC would
        /// silently fall back to a low default-bitrate. <c>-no-scenecut</c> and <c>-forced-idr</c> are the
        /// NVENC-equivalent of <c>-sc_threshold 0</c> and keep the GOP-length fixed, which the HLS-process
        /// (which copies the video-stream without re-encoding it) relies on for constant segment-durations.
        /// </summary>
        private const string _GpuVideoEncoderArgument = "-c:v h264_nvenc -preset p1 -tune ull -rc vbr -cq 23 -b:v 0 -no-scenecut 1 -forced-idr 1 -g 50 -keyint_min 50";
        /// <summary>
        /// Encoder-arguments used when the video is encoded by the CPU.
        /// </summary>
        private const string _CpuVideoEncoderArgument = "-c:v libx264 -preset ultrafast -tune zerolatency -g 50 -keyint_min 50 -sc_threshold 0";
        /// <summary>
        /// Initializes a new instance of <see cref="CameraManagementService"/> with all required dependencies.
        /// </summary>
        /// <param name="businessLogicService">Service used to retrieve camera data.</param>
        /// <param name="logger">Logger used by the base class.</param>
        /// <param name="commandlineParameter">Parsed command-line parameters for the current run.</param>
        /// <param name="processManager">Manager for spawning and tracking external processes.</param>
        /// <param name="runtimeData">Shared in-memory runtime state.</param>
        /// <param name="initializationService">Service that tracks the application initialization state.</param>
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
            Assembly? entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is null || string.IsNullOrEmpty(entryAssembly.Location))
            {
                throw new InvalidOperationException("Could not determine the location of the entry-assembly. The camera-management-service cannot locate its bundled MediaMTX executable.");
            }
            this._EntryAssemblyLocation = entryAssembly.Location;
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
                    this.TerminateProcessesOfNoLongerExistingCameras(cameras.Select(camera => camera.Id).ToHashSet());
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

        /// <summary>
        /// Terminates the media-processes of every camera which is tracked as running but which does not
        /// exist anymore. Without this the MediaMTX- and FFmpeg-processes of a removed camera would keep
        /// running (and keep recording) until the application is restarted, because the reconciliation
        /// itself only iterates over the currently existing cameras.
        /// </summary>
        /// <param name="existingCameraIds">The ids of all currently existing cameras.</param>
        private void TerminateProcessesOfNoLongerExistingCameras(ISet<string> existingCameraIds)
        {
            ICollection<string> trackedCameraIds;
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                trackedCameraIds = this._CameraRuntimeInformation.Keys.ToList();
            }
            foreach (string trackedCameraId in trackedCameraIds)
            {
                if (!existingCameraIds.Contains(trackedCameraId))
                {
                    this._Logger.Log($"Camera {trackedCameraId} does not exist anymore. Terminating its media-processes.", Microsoft.Extensions.Logging.LogLevel.Debug);
                    this.TerminateProcesses(trackedCameraId);
                }
            }
        }

        private void ManageCamera(Camera camera)
        {
            // The lock is deliberately held for the entire reconciliation of a single camera - including
            // the potentially long-running work in StartProcesses (Thread.Sleeps, spawning the
            // MediaMTX/FFmpeg processes and probing the RTSP endpoint via ffprobe). This is intentional and
            // acceptable: the camera-management runs on its own background-thread, cameras are reconciled
            // one after another anyway, and keeping the whole start/stop/detect sequence under one lock
            // guarantees that the tracked runtime-information can never be observed or mutated in a
            // half-started/half-terminated state by a concurrent reader. The added latency for other
            // holders of the lock is a conscious trade-off in favour of this consistency guarantee.
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                if (!this._RuntimeData.GetCameraInternals().ContainsKey(camera.Id))
                {
                    this._RuntimeData.SetCameraInternals(new NotAvailable(camera));
                }
                CameraInternalsBase currentState = this.GetCurrentInternalState(camera);
                this._Logger.Log($"Camera {camera.Id}: detected-state={currentState.GetType().Name}.", Microsoft.Extensions.Logging.LogLevel.Trace);
                try
                {
                    currentState.Accept(new EnsureDesiredConditionIsApplied(this));
                }
                catch (Exception e)
                {
                    this._Logger.Log($"Error while managing camera {camera.Id}. Terminating its media-processes and resetting it to {nameof(NotAvailable)}. It will be reapplied on the next iteration.", e, Microsoft.Extensions.Logging.LogLevel.Debug);
                    this.TerminateProcesses(camera.Id);
                    this._RuntimeData.SetCameraInternals(new NotAvailable(camera));
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

        /// <summary>
        /// Reconciles the actual state of a camera (as detected by <see cref="GetCurrentInternalState"/>)
        /// with the desired state (a camera should always be streaming). Starting and stopping of the
        /// media-processes happens exclusively – and deliberately – here.
        /// </summary>
        private class EnsureDesiredConditionIsApplied : ICameraInternalsBaseVisitor
        {
            private readonly CameraManagementService _CameraManagementService;

            public EnsureDesiredConditionIsApplied(CameraManagementService cameraManagementService)
            {
                this._CameraManagementService = cameraManagementService;
            }

            /// <summary>
            /// The camera is currently streaming. Nothing to do unless its stream-URL changed in the
            /// meantime, in which case the media-processes are restarted with the new configuration.
            /// </summary>
            public void Handle(Available available)
            {
                CameraRuntimeInformation? info = this._CameraManagementService.GetRuntimeInformation(available.Camera.Id);
                if (info is not null && info.Camera.VideoInformation.StreamURL != available.Camera.VideoInformation.StreamURL)
                {
                    this._CameraManagementService.LogDebug($"Stream-URL of camera {available.Camera.Id} changed. Restarting its media-processes.");
                    this._CameraManagementService.TerminateProcesses(available.Camera.Id);
                    this._CameraManagementService.StartProcesses(available.Camera);
                }
            }

            /// <summary>
            /// The camera should be streaming but currently is not available. Any leftover processes are
            /// terminated first, then the media-processes are (re)started.
            /// </summary>
            public void Handle(NotAvailable notAvailable)
            {
                this._CameraManagementService.LogDebug($"Camera {notAvailable.Camera.Id} is not available. (Re)starting its media-processes.");
                this._CameraManagementService.TerminateProcesses(notAvailable.Camera.Id);
                this._CameraManagementService.StartProcesses(notAvailable.Camera);
            }
        }

        /// <summary>
        /// Determines the current internal availability state of a camera by checking whether all of its
        /// associated processes are running and the internal RTSP stream is reachable. This method is a
        /// pure query and does not start, stop or otherwise mutate any process.
        /// </summary>
        /// <param name="camera">The camera whose internal state is evaluated.</param>
        /// <returns>
        /// An <see cref="Available"/> instance when all processes are running and the RTSP endpoint is
        /// reachable; otherwise a <see cref="NotAvailable"/> instance.
        /// </returns>
        private CameraInternalsBase GetCurrentInternalState(Camera camera)
        {
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                if (this._CameraRuntimeInformation.TryGetValue(camera.Id, out CameraRuntimeInformation? info))
                {
                    bool allProcessesRunning = info.GetAllProcesses().All(process => process.IsRunning);
                    string? errorMessage = allProcessesRunning ? null : "not all processes are running";
                    if (allProcessesRunning && IsRtspAvailable(info.MediaMTXURL, out errorMessage))
                    {
                        this._Logger.Log($"Camera {camera.Id} is available internally under \"{info.MediaMTXURL}\".", Microsoft.Extensions.Logging.LogLevel.Trace);
                        return new Available(camera, info.StreamToMediaMTXProcess, info.MediaMTXProcess, info.MediaMTXURL);
                    }
                    else
                    {
                        this._Logger.Log($"Camera {camera.Id} was available internally but is no longer fully operational (all-processes-running={allProcessesRunning}). Reporting it as not-available; its media-processes will be recreated during reconciliation. Details: {errorMessage}", Microsoft.Extensions.Logging.LogLevel.Debug);
                    }
                }
                return new NotAvailable(camera);
            }
        }

        /// <summary>
        /// Returns the tracked runtime information of the given camera, or <c>null</c> if the camera has
        /// no running processes.
        /// </summary>
        private CameraRuntimeInformation? GetRuntimeInformation(string cameraId)
        {
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                return this._CameraRuntimeInformation.TryGetValue(cameraId, out CameraRuntimeInformation? info) ? info : null;
            }
        }

        /// <summary>
        /// Starts MediaMTX, pipes the camera stream into it via FFmpeg, and spawns the screenshot, HLS
        /// and – if the camera is configured for continuous recording – recording processes. On success
        /// the resulting processes are tracked in <see cref="_CameraRuntimeInformation"/> and the camera
        /// is marked as <see cref="Available"/>. If any step fails, every already-started process is
        /// terminated again and the camera is left in the <see cref="NotAvailable"/> state so that the
        /// next iteration retries cleanly.
        /// </summary>
        /// <param name="camera">The camera for which the media-processes are started.</param>
        private void StartProcesses(Camera camera)
        {
            List<ExternalProgramExecutor> startedProcesses = new List<ExternalProgramExecutor>();
            try
            {
                this._Logger.Log($"Starting media-processes for camera {camera.Id} (stream-URL: \"{Misc.Utilities.EscapeBasicAuthPasswords(camera.VideoInformation.StreamURL)}\").", Microsoft.Extensions.Logging.LogLevel.Debug);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(IsRtspAvailable(camera.VideoInformation.StreamURL, out string? errorMessage), $"Camera {camera.Id} is not available: " + errorMessage);
                Thread.Sleep(TimeSpan.FromSeconds(1));//wait until the camera is available again after probing it
                string location = Path.GetDirectoryName(this._EntryAssemblyLocation)!;

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
                ExternalProgramExecutor mediaMTXProcess = this._ProcessManager.GetBackgroundProcess(mediaMTXExecutable, configFileName, mediaMTXFolder, null, $"Media-hub for {camera.Id}", $"MediaHubFor{camera.Id}", false);
                startedProcesses.Add(mediaMTXProcess);
                string url = $"rtsp://127.0.0.1:{mediaMTXPort}/camera_{camera.Id}";
                Thread.Sleep(TimeSpan.FromSeconds(1));
                GRYLibrary.Core.Misc.Utilities.AssertCondition(mediaMTXProcess.IsRunning, () => $"Process terminated unexpectedly with {mediaMTXProcess.ExitCode}.");
                Thread.Sleep(TimeSpan.FromSeconds(1));

                //stream to media hub
                string overlay_folder = $"{this._Constants.GetDataFolder()}/CameraData/{camera.Id}/Overlays";
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(overlay_folder);
                string overlay_file = $"{overlay_folder}\\overlay.png";
                overlay_file = overlay_file.Replace("\\", "/");
                this.CreateOverlayFile(camera, overlay_file);
                bool useGpuAcceleration = this.GpuAccelerationIsAvailable() && !this._CamerasWithDisabledGpuAcceleration.Contains(camera.Id);
                ExternalProgramExecutor streamToMediaMTXProcess = this.StartStreamToMediaHubProcess(camera, overlay_file, url, startedProcesses, ref useGpuAcceleration);
                string inputHardwareAccelerationArgument = GetInputHardwareAccelerationArgument(useGpuAcceleration);

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
                this._Logger.Log($"Provided Camera {camera.Id} internally under \"{url}\".");

                //take screenshots (means: previews)
                // EnsureDirectoryExistsAndIfEmpty clears the folder on every (re)start. This is intentional
                // and wanted: the screenshots are only live previews of the current stream, so no older
                // screenshots (e.g. from a previous stream-URL or a stale session) should ever be shown.
                string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(screenshots_folder);
                string target_file = Path.Combine(screenshots_folder, "frame").Replace("\\", "/");
                // The screenshots are plain JPGs (encoded on the CPU), but the decoding of the incoming
                // stream can still be offloaded to the GPU when one is available. The -reconnect*-options are
                // deliberately absent: they only exist for the HTTP-protocol and never had any effect on this
                // RTSP-input. A dying screenshot-process is detected by GetCurrentInternalState and restarted
                // by the reconciliation instead.
                string ffmpegArgument2 = $"{inputHardwareAccelerationArgument}-rtsp_transport tcp -i \"{url}\" -vf fps=1/2 -qscale:v 2 -strftime 1 \"{target_file}_%Y-%m-%dT%H-%M-%S.jpg\"";
                ExternalProgramExecutor screenshotProcess = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument2, null, null, $"Take screenshots of {camera.Id}", $"TakeScreenshotsOf-{camera.Id}", false);
                startedProcesses.Add(screenshotProcess);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(screenshotProcess.IsRunning, () => $"Process terminated unexpectedly with {screenshotProcess.ExitCode}.");

                //prepare m3u8 stream
                string fradments_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Fragments");
                GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExistsAndIfEmpty(fradments_folder);
                fradments_folder = fradments_folder.Replace("\\", "/");
                uint timeOfFragmentInSeconds = 2;
                uint amountOfFragments = 1;
                string ffmpegArgument3 = $"-rtsp_transport tcp -i \"{url}\" -c:v copy -c:a aac -f hls -hls_time {timeOfFragmentInSeconds} -hls_list_size {amountOfFragments} -hls_flags delete_segments -hls_segment_filename \"{fradments_folder}/segment_%01d.ts\" \"{fradments_folder}/stream.m3u8\"";
                ExternalProgramExecutor m3u8Process = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument3, null, null, $"Provide m3u8-stream {camera.Id}", $"ProvideM3U8Stream-{camera.Id}", false);
                startedProcesses.Add(m3u8Process);
                GRYLibrary.Core.Misc.Utilities.AssertCondition(m3u8Process.IsRunning, () => $"Process terminated unexpectedly with {m3u8Process.ExitCode}.");

                ExternalProgramExecutor? recordProcess = null;
                if (camera.RecordMode is RecordAlways)
                {
                    //record
                    this._Logger.Log($"Start recording {camera.Id}.");
                    string target_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Recordings");
                    GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(target_folder);
                    target_folder = target_folder.Replace("\\", "/");
                    uint videoLengthInSeconds = (uint)Math.Round(this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength.TotalSeconds);
                    string ffmpegArgument4 = $"-rtsp_transport tcp -i \"{url}\" -c copy -f segment -strftime 1 -segment_time {videoLengthInSeconds} -reset_timestamps 1 \"{target_folder}/Camera_{camera.Id}_%Y-%m-%d-%H-%M-%S.mp4\"";
                    recordProcess = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument4, null, null, $"Record camera-stream {camera.Id}", $"RecordCameraStream-{camera.Id}", false);
                    startedProcesses.Add(recordProcess);
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(recordProcess.IsRunning, () => $"Process terminated unexpectedly with {recordProcess.ExitCode}.");
                }

                CameraRuntimeInformation runtimeInformation = new CameraRuntimeInformation(camera, mediaMTXPort, url, mediaMTXProcess, streamToMediaMTXProcess, screenshotProcess, m3u8Process, recordProcess);
                lock (RuntimeData.CameraInternalsRuntimeDataLock)
                {
                    this._CameraRuntimeInformation[camera.Id] = runtimeInformation;
                }
                this._RuntimeData.SetCameraInternals(new Available(camera, streamToMediaMTXProcess, mediaMTXProcess, url));

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            catch (Exception e)
            {
                this._Logger.Log($"Could not start media-processes for {camera.Id}. Terminating the {startedProcesses.Count} already-started process(es) again.", e, Microsoft.Extensions.Logging.LogLevel.Debug);
                this.TerminateProcesses(startedProcesses);
                lock (RuntimeData.CameraInternalsRuntimeDataLock)
                {
                    this._CameraRuntimeInformation.Remove(camera.Id);
                }
                this._RuntimeData.SetCameraInternals(new NotAvailable(camera));
            }
        }

        /// <summary>
        /// Starts the FFmpeg-process which takes the camera-stream, burns the overlay and the timestamp into
        /// it and republishes it to the local media-hub. When <paramref name="useGpuAcceleration"/> is
        /// requested but the process does not survive its start-up, the attempt is repeated on the CPU and
        /// <paramref name="useGpuAcceleration"/> is set to <see langword="false"/> so that the caller
        /// configures the remaining processes of this camera accordingly.
        /// </summary>
        /// <param name="camera">The camera whose stream is republished.</param>
        /// <param name="overlayFile">Absolute path of the overlay-PNG to composite onto the video.</param>
        /// <param name="url">The media-hub-URL the stream is published to.</param>
        /// <param name="startedProcesses">Collects the started processes so that the caller can terminate them on failure.</param>
        /// <param name="useGpuAcceleration">On input whether GPU-acceleration should be attempted; on output whether it is actually in use.</param>
        /// <returns>The running stream-process.</returns>
        private ExternalProgramExecutor StartStreamToMediaHubProcess(Camera camera, string overlayFile, string url, List<ExternalProgramExecutor> startedProcesses, ref bool useGpuAcceleration)
        {
            string purpose = $"StreamToMediaHubFrom-{camera.Id}";
            ExternalProgramExecutor process = this.StartStreamToMediaHubProcessAttempt(camera, overlayFile, url, purpose, useGpuAcceleration);
            startedProcesses.Add(process);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            if (useGpuAcceleration && !process.IsRunning)
            {
                // NVENC was detected as generally usable, yet this particular encoding-session could not be
                // created - the most likely reason is the limit of concurrent NVENC-sessions of consumer-GPUs.
                // Falling back to the CPU for this camera is the only way to get it streaming at all.
                this._Logger.Log($"The GPU-accelerated stream-process of camera {camera.Id} terminated unexpectedly with {process.ExitCode}. Falling back to CPU-encoding for this camera.", Microsoft.Extensions.Logging.LogLevel.Warning);
                this.TerminateProcesses(new ExternalProgramExecutor[] { process });
                startedProcesses.Remove(process);
                this._CamerasWithDisabledGpuAcceleration.Add(camera.Id);
                useGpuAcceleration = false;
                process = this.StartStreamToMediaHubProcessAttempt(camera, overlayFile, url, purpose, false);
                startedProcesses.Add(process);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            ExternalProgramExecutor result = process;
            GRYLibrary.Core.Misc.Utilities.AssertCondition(result.IsRunning, () => $"Process \"{purpose}\" terminated unexpectedly with {result.ExitCode}.");
            return result;
        }

        /// <summary>
        /// Performs a single start-attempt of the stream-process, either on the GPU or on the CPU.
        /// </summary>
        private ExternalProgramExecutor StartStreamToMediaHubProcessAttempt(Camera camera, string overlayFile, string url, string purpose, bool useGpuAcceleration)
        {
            // When a usable GPU is present the video-decoding is offloaded to it (-hwaccel cuda) and the
            // encoding is done by the GPU's NVENC-encoder (h264_nvenc) instead of the CPU-encoder (libx264).
            // The overlay-/drawtext-filters keep running on the CPU (the decoded frames are transferred back
            // to system-memory automatically). If no GPU is available everything runs on the CPU.
            string videoEncoderArgument = useGpuAcceleration ? _GpuVideoEncoderArgument : _CpuVideoEncoderArgument;
            string ffmpegArgument = GetInputHardwareAccelerationArgument(useGpuAcceleration) + "-fflags +genpts -rtsp_transport tcp -use_wallclock_as_timestamps 1  -i \"" + camera.VideoInformation.StreamURL + "\" -loop 1 -i \"" + overlayFile + "\"";
            ffmpegArgument = ffmpegArgument + " -filter_complex \"[0:v][1:v]overlay=0:0:format=auto,drawtext=fontsize=60:fontcolor=white:text='" + camera.Name + " (" + camera.Id + ") %{localtime\\:%Y-%m-%d %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.5:boxborderw=10:x=(w-text_w):y=(h-text_h)\"";//TODO consider camera-timezone in timestamp
            ffmpegArgument = ffmpegArgument + " " + videoEncoderArgument + " -c:a aac -avoid_negative_ts make_zero -vsync vfr -fflags nobuffer -metadata title=\"Camera-" + camera.Id + "\" -f rtsp \"" + url + "\"";//ffmpeg takes the stream and redirects it to mediamtx
            return this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument, null, null, $"Send stream of camera {camera.Id} to media-hub", purpose, false);
        }

        /// <summary>
        /// Returns the FFmpeg input-option which offloads the decoding of the next input to the GPU, or an
        /// empty string when the decoding should happen on the CPU. Note that this must be placed in front of
        /// the <c>-i</c> it belongs to. If the GPU cannot decode the codec of the input, FFmpeg falls back to
        /// software-decoding on its own.
        /// </summary>
        private static string GetInputHardwareAccelerationArgument(bool useGpuAcceleration)
        {
            return useGpuAcceleration ? "-hwaccel cuda " : string.Empty;
        }

        /// <summary>
        /// Terminates every process belonging to the given camera and removes its tracked runtime
        /// information. Does nothing if the camera has no tracked processes.
        /// </summary>
        /// <param name="cameraId">The id of the camera whose processes are terminated.</param>
        private void TerminateProcesses(string cameraId)
        {
            CameraRuntimeInformation? info;
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                this._CameraRuntimeInformation.TryGetValue(cameraId, out info);
            }
            if (info is not null)
            {
                this.LogDebug($"Terminating media-processes of camera {cameraId}.");
                this.TerminateProcesses(info.GetAllProcesses());
                lock (RuntimeData.CameraInternalsRuntimeDataLock)
                {
                    this._CameraRuntimeInformation.Remove(cameraId);
                }
            }
        }

        /// <summary>
        /// Terminates the given set of processes, ignoring (but logging) errors for each individual
        /// process so that a failure to terminate one process does not prevent terminating the others.
        /// </summary>
        /// <param name="processes">The processes to terminate.</param>
        private void TerminateProcesses(IEnumerable<ExternalProgramExecutor> processes)
        {
            foreach (ExternalProgramExecutor process in processes)
            {
                try
                {
                    if (process.IsRunning)
                    {
                        process.Terminate();
                    }
                }
                catch (Exception exception)
                {
                    this.LogDebug($"Error while terminating a media-process.", exception);
                }
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
            // File.Create (FileMode.Create) truncates an already existing file to zero length first, so no
            // trailing bytes of a previously larger overlay-PNG can remain and corrupt the new file.
            using FileStream stream = System.IO.File.Create(overlayFile);
            data.SaveTo(stream);
            this._Logger.Log($"Created overlay-file for camera {camera.Id} ({width}x{height}, {polygons.Count} polygon(s)) at \"{overlayFile}\".", Microsoft.Extensions.Logging.LogLevel.Debug);
        }

        /// <summary>
        /// Returns the next free port number from the sequentially allocated, inclusive port range
        /// [<see cref="_LastUsedPortRangeBegin"/>, <see cref="_LastUsedPortRangeEnd"/>]. The very first
        /// port ever handed out is <see cref="_LastUsedPortRangeBegin"/>; afterwards the range is walked
        /// upwards and wraps back around to <see cref="_LastUsedPortRangeBegin"/> once
        /// <see cref="_LastUsedPortRangeEnd"/> has been reached. Ports that are currently in use are
        /// skipped.
        /// </summary>
        /// <returns>A port number that is currently free.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no free port could be found.</exception>
        private ushort GetNewFreePort()
        {
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                int amountOfPortsInRange = _LastUsedPortRangeEnd - _LastUsedPortRangeBegin + 1;
                for (int attempt = 0; attempt < amountOfPortsInRange; attempt++)
                {
                    // Advance to the next candidate. On the first call (no port allocated yet) and whenever
                    // the end of the range was reached (or the value somehow left the range) we (re)start at
                    // the begin; otherwise we step one port further.
                    if (this._LastUsedPort is null || this._LastUsedPort.Value >= _LastUsedPortRangeEnd || this._LastUsedPort.Value < _LastUsedPortRangeBegin)
                    {
                        this._LastUsedPort = _LastUsedPortRangeBegin;
                    }
                    else
                    {
                        this._LastUsedPort = (ushort)(this._LastUsedPort.Value + 1);
                    }
                    if (PortIsFree(this._LastUsedPort.Value))
                    {
                        return this._LastUsedPort.Value;
                    }
                }
                throw new InvalidOperationException($"No free port available in the range [{_LastUsedPortRangeBegin}, {_LastUsedPortRangeEnd}].");
            }
        }

        /// <summary>
        /// Determines whether the given TCP port is currently free on the local machine by attempting to
        /// bind a listener to it on all interfaces.
        /// </summary>
        /// <param name="port">The TCP port to check.</param>
        /// <returns><see langword="true"/> if the port can currently be bound; otherwise <see langword="false"/>.</returns>
        public static bool PortIsFree(ushort port)
        {
            TcpListener? listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            finally
            {
                listener?.Stop();
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
        /// <summary>
        /// Determines - once per process-lifetime, then cached - whether FFmpeg can use NVIDIA-GPU-
        /// acceleration (NVENC) on this machine. The result is used to decide whether the FFmpeg-processes
        /// are configured to run on the GPU or on the CPU. Only NVIDIA-GPUs are supported for now.
        /// </summary>
        /// <returns><see langword="true"/> if GPU-acceleration is available; otherwise <see langword="false"/>.</returns>
        private bool GpuAccelerationIsAvailable()
        {
            lock (this._GpuDetectionLock)
            {
                if (this._GpuAccelerationAvailable is null)
                {
                    this._GpuAccelerationAvailable = DetectNvencAvailability();
                    if (this._GpuAccelerationAvailable.Value)
                    {
                        this._Logger.Log("Detected a usable NVIDIA-GPU. FFmpeg-processes will use NVENC-based hardware-acceleration.", Microsoft.Extensions.Logging.LogLevel.Information);
                    }
                    else
                    {
                        this._Logger.Log("No usable GPU-acceleration was detected. FFmpeg-processes will run on the CPU.", Microsoft.Extensions.Logging.LogLevel.Information);
                    }
                }
                return this._GpuAccelerationAvailable.Value;
            }
        }

        /// <summary>
        /// Probes whether the NVIDIA-NVENC-encoder is actually usable by letting FFmpeg encode a few tiny
        /// frames with exactly the same encoder-arguments the camera-processes use. This is the only reliable
        /// check because the encoder can be compiled into FFmpeg while still failing at runtime when no
        /// NVIDIA-GPU or driver is present, and because some of the used options (e.g. the <c>p1</c>-preset)
        /// only exist in newer FFmpeg-versions and would otherwise let the camera-process die instead.
        /// </summary>
        /// <returns><see langword="true"/> if the test-encode succeeds; otherwise <see langword="false"/>.</returns>
        private static bool DetectNvencAvailability()
        {
            try
            {
                ExternalProgramExecutor e = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
                {
                    Program = "ffmpeg",
                    Argument = $"-hide_banner -loglevel error -f lavfi -i color=black:s=256x256:d=0.1 {_GpuVideoEncoderArgument} -f null -",
                    TimeoutInMilliseconds = (int)TimeSpan.FromSeconds(20).TotalMilliseconds,
                    Verbosity = Verbosity.Quiet,
                });
                e.Configuration.WaitingState = new RunSynchronously()
                {
                    ThrowErrorIfExitCodeIsNotZero = false,
                };
                e.Run();
                return e.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Terminates the media-processes of every managed camera and clears the tracked runtime
        /// information. Used on shutdown so that no MediaMTX/FFmpeg processes are left behind.
        /// </summary>
        private void TerminateAllProcesses()
        {
            ICollection<string> cameraIds;
            lock (RuntimeData.CameraInternalsRuntimeDataLock)
            {
                cameraIds = this._CameraRuntimeInformation.Keys.ToList();
            }
            foreach (string cameraId in cameraIds)
            {
                this.TerminateProcesses(cameraId);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TerminateAllProcesses();
            }
            base.Dispose(disposing);
        }
    }
}
