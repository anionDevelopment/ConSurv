using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ConSurvBackend.Core.Miscellaneous;
using GRYLibrary.Core.APIServer.Services.Res;

namespace ConSurvBackend.Core.Services
{
    public sealed class RTSPManager : IRTSPManager, IDisposable
    {
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly Dictionary<string/*cameraid*/, RecordInformation> _RecordingProcesses = new Dictionary<string, RecordInformation>();
        private readonly IGRYLog _Log;
        private readonly ITimeService _TimeService;
        private readonly IApplicationConstants _Constants;
        private readonly IProcessManager _ProcessManager;
        private readonly IGeneralResourceLoader _GeneralResourceLoader;

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
        public RTSPManager(IGRYLog log, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, ITimeService timeService, IApplicationConstants constants, IProcessManager processManager, IGeneralResourceLoader generalResourceLoader)

        {
            this._Log = log;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._TimeService = timeService;
            this._Constants = constants;
            this._ProcessManager = processManager;
            this._GeneralResourceLoader = generalResourceLoader;
        }

        #region public functions

        public bool IsAvailable(Camera camera)
        {
            try
            {
                return this.GetPreview(camera, default, default).success;
            }
            catch
            {
                return false;
            }
        }
        public (bool success, byte[] picture) GetPreview(Camera camera, uint? maximalHeight, uint? maximalWidth)
        {
            lock (camera.Id)
            {
                string tempFile = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
                try
                {
                    uint maximalHeightValue = maximalHeight ?? 75;
                    uint maximalWidthValue = maximalWidth ?? 100;
                    bool logToConsole = this._Constants.Environment is Development;
                    using ExternalProgramExecutor process = new ExternalProgramExecutor("ffmpeg", $"-i {camera.VideoInformation.StreamURL} -vframes 1 -s {maximalWidthValue}x{maximalHeightValue} {tempFile}");
                    process.Run();
                    if (process.ExitCode != 0)
                    {
                        _Log.LogProgramOutput($"Generate-preview-process exited with exitcode {process.ExitCode}.", process.AllStdOutLines, process.AllStdErrLines, LogLevel.Warning);
                    }
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(process.ExitCode == 0);
                    return (true, File.ReadAllBytes(tempFile));
                }
                catch (Exception e)
                {
                    return (false, _GeneralResourceLoader.GetResource("NoPreviewAvailablePicture.jpg"));
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
            this._Log.Log($"Start recording on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is RecordAlways);
            this._RecordingProcesses[camera.Id] = new RecordInformation(true, null, null, camera, camera.RecordMode);

            Thread thread = new Thread(() => this.RecordAlwaysRunner(camera, camera.VideoInformation.StreamURL, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TargetFolder, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TimeInUTC));
            thread.Start();
            this._RecordingProcesses[camera.Id].Thread = thread;
        }

        private void StartToRecordOnMovements(Camera camera)
        {
            this._Log.Log($"Start recording movements on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is RecordOnMovements);
            this._RecordingProcesses[camera.Id] = new RecordInformation(true, null, null, camera, camera.RecordMode);

            throw new NotImplementedException();
        }

        private void StopRecording(Camera camera)
        {
            this._Log.Log($"Stop recording on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is NoRecording);
            this.TerminateProcess(this._RecordingProcesses[camera.Id].Process);
        }

        #endregion

        private void TerminateProcess(Process p)
        {
            p.Kill();
        }

        private void RecordAlwaysRunner(Camera camera, string streamURL, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            bool enabled = true;
            while (enabled)
            {
                try
                {
                    string targetFile = Miscellaneous.Utilities.GetVideoTargetFile(targetFolder, camera.Id, timeInUTC, this._TimeService);
                    GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(Path.GetDirectoryName(targetFile)!);
                    Process process = Utilities.GetBackgroundProcess("ffmpeg", $"-i {streamURL} -t {(uint)Math.Round(videoLength.TotalSeconds, 0)} -c:v copy -c:a aac {targetFile}", null, _Constants.GetConfigurationFolder(), null);
                    lock (camera.Id)
                    {
                        GRYLibrary.Core.Misc.Utilities.AssertCondition(this._RecordingProcesses[camera.Id].Process == null);
                        this._RecordingProcesses[camera.Id].Process = process;
                        if (!this.IsAvailable(camera))
                        {
                            //TODO throw exception or try again later
                        }
                        //drawing a timestamp into the video would be possible here using an argument like '-i {streamURL} -vf "drawtext=fontfile=roboto.ttf:fontsize=36:fontcolor=yellow:text='%{pts\:gmtime\:1575526882\:%A, %d, %B %Y %I\\\:%M\\\:%S %p}'"' but this can not be used together with coping the stream (see https://stackoverflow.com/a/53526514/3905529 ) so this decreases the performance/quality significantly.
                    }
                    this._ProcessManager.RegisterProcess(process);
                    process.WaitForExit();
                    lock (camera.Id)
                    {
                        if (process.ExitCode != 0)
                        {
                            this._Log.Log($"Record-process exited with exitcode {process.ExitCode}.", LogLevel.Warning);
                        }
                        process.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    this._Log.Log($"Error while record-loop for camera with id '{camera.Id}'.", LogLevel.Warning, ex, null);
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));//prevent hig cpu-usage
                }
                finally
                {
                    lock (camera.Id)
                    {
                        this._RecordingProcesses[camera.Id].Process = null;
                    }
                }
            }
            lock (camera.Id)
            {
                this._RecordingProcesses[camera.Id].Thread = null;
            }
        }
        public void Dispose()
        {
            //TODO call EnsureNotRecording for all cameras
        }
    }
}
