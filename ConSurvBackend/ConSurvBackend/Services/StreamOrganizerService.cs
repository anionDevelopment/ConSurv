using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using OperatingSystem = GRYLibrary.Core.OperatingSystem.OperatingSystem;

namespace ConSurvBackend.Core.Services
{
    public class StreamOrganizerService : IStreamOrganizerService
    {
        private static readonly object _Lock = new object();
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly IDictionary<string, StreamOrganizationDataset> _Cameras;
        private readonly IApplicationConstants _ApplicationConstants;
        private readonly IGRYLog _Log;
        private readonly IProcessManager _ProcessManager;
        private readonly ITimeService _TimeService;
        public StreamOrganizerService(IApplicationConstants applicationConstants, IGRYLog log, IProcessManager processManager, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration, ITimeService timeService)
        {
            this._ApplicationConstants = applicationConstants;
            this._Log = log;
            this._Cameras = new Dictionary<string, StreamOrganizationDataset>();
            this._ProcessManager = processManager;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._TimeService = timeService;
        }
        public void InitializeCameraOrganization()
        {
            string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string mediaMTXFolder = Path.Combine(location, "MediaMTX");
            string mediaMTXExecutable = Path.Combine(mediaMTXFolder, "mediamtx");
            if (OperatingSystem.GetCurrentOperatingSystem() is GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems.Windows)
            {
                mediaMTXExecutable = mediaMTXExecutable + ".exe";
            }
            ExternalProgramExecutor process = this._ProcessManager.GetBackgroundProcess(mediaMTXExecutable, string.Empty, mediaMTXFolder, null, $"Media-hub", $"MediaHub", false);
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        public void OrganizeCamera(Camera camera)
        {
            lock (_Lock)
            {
                bool isCurrentlyManaged = this._Cameras.ContainsKey(camera.Id);
                bool shouldBeManaged = true;  //TODO add something like "alwaysavailable" to mediamtx-config like described here https://github.com/bluenviron/mediamtx/issues/2214 to have a defined behavior when the camera is not available so that there is always a stream available for the camera even if it does only show a dummy-picture when the camera is not available
                if (isCurrentlyManaged != shouldBeManaged)
                {
                    if (shouldBeManaged)
                    {
                        this.ProvideStream(camera);
                        this.StartRecordingOfCamera(camera.Id);
                        this.StartStreamOfCamera(camera.Id);
                    }
                    else
                    {
                        StreamOrganizationDataset entry = this._Cameras[camera.Id];
                        if (entry.Process.IsRunning)
                        {
                            entry.Process.Terminate();
                        }
                        this._Cameras.Remove(camera.Id);
                    }
                }
            }
        }

        private void StartRecordingOfCamera(string cameraId)
        {
            TimeSpan timeOfEveryFile = this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.VideoLength;
            if (this._ApplicationConstants.Environment is Development)
            {
                timeOfEveryFile = TimeSpan.FromMinutes(1);
            }
            string targetFolder = this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration.TargetFolder;
            string targetFolderFinal = Path.Combine(targetFolder, cameraId).Replace(@"\", "/");
            GUtilities.EnsureDirectoryExists(targetFolderFinal);
            string argument = $"-i rtsp://localhost:8554/Stream_{cameraId} -acodec copy -vcodec copy -f segment -segment_time {(int)Math.Round(timeOfEveryFile.TotalSeconds, 0)} -strftime 1 -reset_timestamps 1 {targetFolderFinal}/Camera-{cameraId}_%Y-%m-%d_%H-%M-%S.avi";
            this._ProcessManager.GetBackgroundProcess("ffmpeg", argument, null, null, $"Record {cameraId}", $"Record-{cameraId}", false);
        }

        private void ProvideStream(Camera camera)
        {
            string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ushort port = this.GetNextFreePort();
            string path = $"Stream_{camera.Id}";
            string fontfile = Path.Combine(location, "Fonts", "Noto", "NotoSans_Condensed-Regular.ttf");
            GUtilities.AssertCondition(File.Exists(fontfile), "Font-file does not exist.");
            ExternalProgramExecutor process = this._ProcessManager.GetBackgroundProcess("ffmpeg", "-rtsp_transport tcp -i " + camera.VideoInformation.StreamURL + " -vf \"drawtext=fontfile='" + fontfile + "':fontsize=60:fontcolor=white:text='%{localtime\\:%Y-%m-%d %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.5:boxborderw=10:x=(w-text_w):y=(h-text_h)\" -c:v libx264 -c:a aac -vsync passthrough -fflags nobuffer -f rtsp " + this.GetStreamURL(camera.Id), null, null, $"Send stream of camera {camera.Id} to media-hub", $"StreamToMediaHubFrom-{camera.Id}", false);
            this._Cameras[camera.Id] = new StreamOrganizationDataset()
            {
                Camera = camera,
                Path = path,
                Port = port,
            };
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        private void StartStreamOfCamera(string cameraId)
        {
            string outputDir = Path.Combine(this._ApplicationConstants.GetDataFolder(), "Streaming", cameraId).Replace(@"\", "/");
            GUtilities.EnsureDirectoryDoesNotExist(outputDir);
            GUtilities.EnsureDirectoryExists(outputDir);
            string rtspUrl = this.GetStreamURL(cameraId);
            string args = $"-i {rtspUrl} -c:v libx264 -c:a aac -f hls -hls_time 2 -hls_list_size 5 -hls_flags delete_segments {outputDir}/stream.m3u8";
            this._ProcessManager.GetBackgroundProcess("ffmpeg", args, Environment.CurrentDirectory, (process) => { }, $"Streaming of camera {cameraId}", $"Stream-{cameraId}", false);
            //TODO check if wait a few seconds is required here
        }
        private ushort GetNextFreePort()
        {
            ushort port = 10000;
            while (port < ushort.MaxValue)
            {
                if (this.PortIsFree(port))
                {
                    return port;
                }
                port = (ushort)(port + 1);
            }
            throw new InternalAlgorithmException($"Can not find a free port.");
        }

        private bool PortIsFree(ushort port)
        {
            foreach (KeyValuePair<string, StreamOrganizationDataset> camera in this._Cameras)
            {
                if (camera.Value.Port == port)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsAvailable(string cameraId)
        {
            lock (_Lock)
            {
                return this._Cameras.ContainsKey(cameraId);
            }
        }

        public string GetStreamURL(string cameraId)
        {
            lock (_Lock)
            {
                return "rtsp://localhost:8554/Stream_" + cameraId;
            }
        }
    }
}
