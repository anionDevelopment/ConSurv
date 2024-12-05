using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ConSurvBackend.Core.Services
{
    public class RTSPManager : IRTSPManager
    {
        private record RecordInformation
        {
            public bool Enabled;
            public Process? Process;

            public RecordInformation(bool enabled, Process? process)
            {
                this.Enabled = enabled;
                this.Process = process;
            }
        }
        private readonly Dictionary<string, RecordInformation> _RecordingProcesses = new Dictionary<string, RecordInformation>();
        private readonly IGRYLog _Log;
        public RTSPManager(IGRYLog log)
        {
            this._Log = log;
        }

        public void EnsureRecordingAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            Thread thread = new Thread(() => this.RecordLoop(camera.Id, camera.VideoInformation.StreamURL, targetFolder, videoLength, timeInUTC));
            thread.Start();
        }

        public void EnsureNotRecording(string cameraId)
        {
            lock (cameraId)
            {
                RecordInformation ri = this.GetRecordInformation(cameraId);
                this.SetRecordInformation(cameraId, new RecordInformation(false, ri.Process));
                if (ri.Process != null)
                {
                    if (!ri.Process.HasExited)
                    {
                        this.TerminateProcessClean(ri.Process);
                    }
                }
            }
        }
        internal void TerminateProcessClean(Process p)
        {
            p.StandardInput.Close();
        }

        private void RecordLoop(string cameraId, string streamURL, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            while (this.GetRecordInformation(cameraId).Enabled)
            {
                Process? process = null;
                try
                {
                    lock (cameraId)
                    {
                        //TODO check if camera is available
                        process = new Process();
                        if (this.GetRecordInformation(cameraId).Enabled)
                        {
                            this.SetRecordInformation(cameraId, new RecordInformation(true, process));
                        }
                        else
                        {
                            return;
                        }
                    }

                    process.StartInfo.FileName = "ffmpeg";
                    string targetFile = Miscellaneous.Utilities.GetVideoTargetFile(targetFolder, cameraId, timeInUTC);
                    GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(Path.GetDirectoryName(targetFile));
                    process.StartInfo.Arguments = $"-i {streamURL} -t {(uint)Math.Round(videoLength.TotalSeconds, 0)} -vcodec copy -acodec copy {targetFile}";
                    //drawing a timestamp into the video would be possible here using an argument like '-i {streamURL} -vf "drawtext=fontfile=roboto.ttf:fontsize=36:fontcolor=yellow:text='%{pts\:gmtime\:1575526882\:%A, %d, %B %Y %I\\\:%M\\\:%S %p}'"' but this can not be used together with coping the stream (see https://stackoverflow.com/a/53526514/3905529 ) so this decreases the performance/quality significantly.
                    process.Start();
                    process.WaitForExit();
                    lock (cameraId)
                    {
                        process.Dispose();
                        this.SetRecordInformation(cameraId, new RecordInformation(true, null));
                    }
                }
                catch (Exception ex)
                {
                    //TODO
                    Thread.Sleep(TimeSpan.FromSeconds(2));//prevent hig cpu-usage
                }

            }
        }

        #region thread safe record information access

        private void SetRecordInformation(string cameraId, RecordInformation recordInformation)
        {
            lock (cameraId)
            {
                this._RecordingProcesses[cameraId] = recordInformation;
            }
        }
        private RecordInformation GetRecordInformation(string cameraId)
        {
            lock (cameraId)
            {
                return this._RecordingProcesses[cameraId];
            }
        }

        public byte[] GetPreview(string id, string streamURL)
        {
            string tempFile = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
            try
            {
                using (Process process = new Process())
                {

                    process.StartInfo.FileName = "ffmpeg";
                    process.StartInfo.Arguments = $"-i {streamURL} -vframes 1 {tempFile}";
                    process.Start();
                    process.WaitForExit();
                }
                return File.ReadAllBytes(tempFile);
            }
            finally
            {
                GRYLibrary.Core.Misc.Utilities.EnsureFileDoesNotExist(tempFile);
            }
        }

        public void EnsureRecordingOnMovementsAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
