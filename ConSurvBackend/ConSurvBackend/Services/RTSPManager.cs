using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ConSurvBackend.Core.Services
{
    public sealed class RTSPManager : IRTSPManager, IDisposable
    {
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly Dictionary<string/*cameraid*/, RecordInformation> _RecordingProcesses = new Dictionary<string, RecordInformation>();
        private readonly IGRYLog _Log;
        private readonly IAuditLog _AuditLog;
        private readonly ITimeService _TimeService;
        private readonly IApplicationConstants _Constants;
        private readonly IGeneralResourceLoader _GeneralResourceLoader;
        private readonly IStreamOrganizerService _StreamOrganizerService;
        private readonly IProcessManager _ProcessManager;

        public RTSPManager(IGRYLog log, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, ITimeService timeService, IApplicationConstants constants, IGeneralResourceLoader generalResourceLoader, IAuditLog auditLog, IStreamOrganizerService streamOrganizerService, IProcessManager processManager)

        {
            this._Log = log;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._TimeService = timeService;
            this._Constants = constants;
            this._GeneralResourceLoader = generalResourceLoader;
            this._AuditLog = auditLog;
            this._StreamOrganizerService = streamOrganizerService;
            this._ProcessManager = processManager;
        }

        private record RecordInformation
        {
            public bool Enabled;
            public Process? Process;
            public Thread? Thread;
            public Camera Camera;
            public RecordMode LastSetRecordMode;

            public RecordInformation(bool enabled, Process? process, Thread? thread, Camera camera, RecordMode lastSetRecordMode)
            {
                this.Enabled = enabled;
                this.Process = process;
                this.Thread = thread;
                this.Camera = camera;
                this.LastSetRecordMode = lastSetRecordMode;
            }
        }
        #region public functions

        public bool IsAvailable(Camera camera)
        {
            try
            {
                return this.GetPreviewDirectlyFromCamera(camera, default, default, false, this._Log).success;
            }
            catch
            {
                return false;
            }
        }
        private (bool success, byte[] picture) GetPreviewDirectlyFromCamera(Camera camera, uint? maximalHeight, uint? maximalWidth, bool logFail, IGRYLog log)
        {
            lock (camera.Id)
            {
                string tempFile = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
                try
                {
                    uint maximalHeightValue = maximalHeight ?? 75;
                    uint maximalWidthValue = maximalWidth ?? 100;
                    bool logToConsole = this._Constants.Environment is Development;
                    string argument= $"-i {this._StreamOrganizerService.GetStreamURL(camera.Id)} -vframes 1 -s {maximalHeightValue}x{maximalWidthValue} {tempFile}";
                    ExternalProgramExecutor process = this._ProcessManager.GetBackgroundProcess("ffmpeg", argument, null, null, "Generate preview", $"PreviewFor-{camera.Id}", true);
                    if (process.ExitCode != 0)
                    {
                        throw new InternalAlgorithmException(GRYLog.FormatProgramOutput($"Generate-preview-process exited with exitcode {process.ExitCode}.", process.AllStdOutLines, process.AllStdErrLines));
                    }
                    return (true, File.ReadAllBytes(tempFile));
                }
                catch (Exception e)
                {
                    if (logFail)
                    {
                        bool verbose = false;//can be changed to true temporary for debugging purposes
                        if (verbose)
                        {
                            log.LogException(e, $"Error while generating preview for camera with id '{camera.Id}'.", LogLevel.Warning);
                        }
                        else
                        {
                            log.Log($"Error while generating preview for camera with id '{camera.Id}'.", LogLevel.Warning);
                        }
                    }
                    return (false, this._GeneralResourceLoader.GetResource("NoPreviewAvailablePicture.jpg"));
                }
                finally
                {
                    GRYLibrary.Core.Misc.Utilities.EnsureFileDoesNotExist(tempFile);
                }
            }
        }

        public void EnsureRecordingAlwaysAsync(Camera camera)
        {
            lock (camera.Id)
            {
                if (!this.IsRecordingAlways(camera))
                {
                    this.StartToRecordAlways(camera);
                }
            }
        }

        public void EnsureRecordingOnMovementsAsync(Camera camera)
        {
            lock (camera.Id)
            {
                if (!this.IsRecordingOnMovements(camera))
                {
                    this.StartToRecordOnMovements(camera);
                }

            }
        }

        public void EnsureNotRecording(Camera camera)
        {
            lock (camera.Id)
            {
                if (!this.IsNotRecording(camera))
                {
                    if (this._RecordingProcesses.TryGetValue(camera.Id, out RecordInformation? value) && value.Process != null)
                    {
                        this.StopRecording(camera);
                    }
                }
            }
        }

        #endregion

        #region Check-functions

        private bool IsRecordingAlways(Camera camera)
        {
            if (this._RecordingProcesses.TryGetValue(camera.Id, out RecordInformation? value))
            {
                return value.LastSetRecordMode is RecordAlways;
            }
            else
            {
                return false;
            }
        }
        private bool IsRecordingOnMovements(Camera camera)
        {
            if (this._RecordingProcesses.TryGetValue(camera.Id, out RecordInformation? value))
            {
                return value.LastSetRecordMode is RecordOnMovements;
            }
            else
            {
                return false;
            }
        }

        private bool IsNotRecording(Camera camera)
        {
            if (this._RecordingProcesses.TryGetValue(camera.Id, out RecordInformation? value))
            {
                return value.LastSetRecordMode is NoRecording;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region On-record-state-handler

        private void StartToRecordAlways(Camera camera)
        {
            this._AuditLog.AuditLogger.Log($"Start recording on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is RecordAlways);
            this._RecordingProcesses[camera.Id] = new RecordInformation(true, null, null, camera, camera.RecordMode);

            Thread thread = new Thread(() => this.RecordAlwaysRunner(camera, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TargetFolder, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TimeInUTC));
            thread.Start();
            this._RecordingProcesses[camera.Id].Thread = thread;
        }

        private void StartToRecordOnMovements(Camera camera)
        {
            this._AuditLog.AuditLogger.Log($"Start recording movements on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is RecordOnMovements);
            this._RecordingProcesses[camera.Id] = new RecordInformation(true, null, null, camera, camera.RecordMode);

            throw new NotImplementedException();
        }

        private void StopRecording(Camera camera)
        {
            this._AuditLog.AuditLogger.Log($"Stop recording on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is NoRecording);
            this.TerminateProcess(this._RecordingProcesses[camera.Id].Process);
        }

        #endregion

        private void TerminateProcess(Process p)
        {
            p.Kill();
        }

        private void RecordAlwaysRunner(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            //obsolete:
            bool enabled = true;
            while (enabled)
            {
                try
                {
                    return;
                    lock (camera.Id)
                    {
                        bool addSleep = false;
                        if (addSleep)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1.1));
                        }
                        if (this.IsAvailable(camera))
                        {
                            if (this._Constants.Environment is Development)
                            {
                                videoLength = TimeSpan.FromSeconds(20);//more useful for debugging
                            }
                            string targetFolderFinal = Path.Combine(targetFolder, camera.Id).Replace(@"\", "/");
                            GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(targetFolderFinal);
                            string targetFile = $"{targetFolderFinal}/{Utilities.GetVideoTargetFile(camera.Id, timeInUTC, this._TimeService)}";
                            GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(GRYLibrary.Core.Misc.Utilities.GetValue(Path.GetDirectoryName(targetFile)));
                            string streamURL = this._StreamOrganizerService.GetStreamURL(camera.Id);
                            string ffmpegArgument = $"-i {streamURL} -t {(uint)Math.Round(videoLength.TotalSeconds, 0)} -c:v copy -c:a aac {targetFile}";
                            using ExternalProgramExecutor process = this._ProcessManager.GetBackgroundProcess("ffmpeg", ffmpegArgument, null, null, $"Record camera \"{camera.Name}\" (Id: {camera.Id})", $"Record-{camera.Id}", true);
                            process.WaitUntilTerminated();//wait for exit because this function will already be executed in a background-thread.
                            if (process.ExitCode != 0)
                            {
                                this._Log.Log(GRYLog.FormatProgramOutput($"Process for recording camera {camera.Id} exited due to a problem: ", process.AllStdOutLines, process.AllStdErrLines), LogLevel.Warning);
                            }
                        }
                        else
                        {
                            this._Log.Log($"Camera '{camera.Id}' is not available.", LogLevel.Warning);
                            Thread.Sleep(TimeSpan.FromSeconds(20));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._Log.Log($"Error while record-loop for camera with id '{camera.Id}'.", LogLevel.Error, ex, null);
                    Thread.Sleep(TimeSpan.FromSeconds(2));//prevent hig cpu-usage for error-loops
                }
            }
        }
        public void Dispose()
        {
            //TODO call EnsureNotRecording for all cameras
        }
    }
}
