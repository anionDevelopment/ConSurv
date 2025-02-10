using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
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
    public class RTSPManager : IRTSPManager
    {
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly Dictionary<string/*cameraid*/, RecordInformation> _RecordingProcesses = new Dictionary<string, RecordInformation>();
        private readonly IGRYLog _Log;
        private readonly ITimeService _TimeService;
        private record RecordInformation
        {
            public bool Enabled;
            public ExternalProgramExecutor? Process;
            public Thread? Thread;
            public Camera Camera;
            public RecordMode LastSetRecordMode;

            public RecordInformation(bool enabled, ExternalProgramExecutor? process, Thread? thread, Camera camera, RecordMode lastSetRecordMode)
            {
                this.Enabled = enabled;
                this.Process = process;
                this.Thread = thread;
                this.Camera = camera;
                this.LastSetRecordMode = lastSetRecordMode;
            }
        }
        public RTSPManager(IGRYLog log, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, ITimeService timeService)
        {
            this._Log = log;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._TimeService = timeService;
        }

        #region public functions

        public bool IsAvailable(Camera camera)
        {
            try
            {
                return this.GetPreview(camera).success;
            }
            catch
            {
                return false;
            }
        }
        public (bool success, byte[] picture) GetPreview(Camera camera)
        {
            lock (camera.Id)
            {
                string tempFile = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "ffmpeg";
                        process.StartInfo.Arguments = $"-i {camera.VideoInformation.StreamURL} -vframes 1 {tempFile}";
                        process.StartInfo.RedirectStandardInput = true;//prevent output to console
                        process.StartInfo.RedirectStandardError = true;//prevent output to console
                        process.Start();
                        process.WaitForExit();
                    }
                    return (true, File.ReadAllBytes(tempFile));
                }
                catch
                {
                    throw new NotImplementedException();//TODO return something like (false, preview-not-available-dummy-picture)
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
                    if (this._RecordingProcesses.TryGetValue(camera.Id, out RecordInformation? value) && value.Process != null && value.Process._Process != null)
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
            _Log.Log($"Start recording on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is RecordAlways);
            this._RecordingProcesses[camera.Id] = new RecordInformation(true, null, null, camera, camera.RecordMode);

            Thread thread = new Thread(() => this.RecordAlwaysRunner(camera, camera.VideoInformation.StreamURL, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TargetFolder, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TimeInUTC));
            thread.Start();
            this._RecordingProcesses[camera.Id].Thread = thread;
        }

        private void StartToRecordOnMovements(Camera camera)
        {
            _Log.Log($"Start recording movements on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is RecordOnMovements);
            this._RecordingProcesses[camera.Id] = new RecordInformation(true, null, null, camera, camera.RecordMode);

            throw new NotImplementedException();
        }

        private void StopRecording(Camera camera)
        {
            _Log.Log($"Stop recording on camera {camera.Id}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(camera.RecordMode is NoRecording);
            this.TerminateProcess(this._RecordingProcesses[camera.Id].Process._Process!);
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
                    ExternalProgramExecutor process = new ExternalProgramExecutor("ffmpeg", $"-i {streamURL} -t {(uint)Math.Round(videoLength.TotalSeconds, 0)} -c:v copy -c:a aac {targetFile}");
                    lock (camera.Id)
                    {
                        GRYLibrary.Core.Misc.Utilities.AssertCondition(this._RecordingProcesses[camera.Id].Process == null);
                        this._RecordingProcesses[camera.Id].Process = process;
                        process.LogObject = GeneralLogger.NoLogAsGRYLog();

                        process.Configuration.WaitingState = new RunSynchronously();
                        if (!this.IsAvailable(camera))
                        {
                            //TODO throw exception or try again later
                        }
                        //drawing a timestamp into the video would be possible here using an argument like '-i {streamURL} -vf "drawtext=fontfile=roboto.ttf:fontsize=36:fontcolor=yellow:text='%{pts\:gmtime\:1575526882\:%A, %d, %B %Y %I\\\:%M\\\:%S %p}'"' but this can not be used together with coping the stream (see https://stackoverflow.com/a/53526514/3905529 ) so this decreases the performance/quality significantly.
                    }
                    process.Run();
                    lock (camera.Id)
                    {
                        if (process.ExitCode != 0)
                        {
                            this._Log.LogProgramOutput($"ffmpeg exited with exitcode {process.ExitCode}", process.AllStdOutLines, process.AllStdErrLines, LogLevel.Warning);
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
    }
}
