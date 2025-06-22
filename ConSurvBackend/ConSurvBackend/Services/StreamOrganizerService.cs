using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.OperatingSystem;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ConSurvBackend.Core.Services
{
    public class StreamOrganizerService : IStreamOrganizerService
    {
        private static readonly object _Lock = new object();
        private readonly IDictionary<string, StreamOrganizationDataset> _Cameras;
        private readonly IApplicationConstants _ApplicationConstants;
        private readonly IGRYLog _Log;
        public StreamOrganizerService(IApplicationConstants applicationConstants, IGRYLog log)
        {
            this._ApplicationConstants = applicationConstants;
            this._Log = log;
            this._Cameras = new Dictionary<string, StreamOrganizationDataset>();
        }
        public void OrganizeCamera(Camera camera)
        {
            lock (_Lock)
            {
                bool isCurrentlyManaged = this._Cameras.ContainsKey(camera.Id);
                bool shouldBeManaged = true;  //TODO add something like "alwaysavailable" to mediamtx-config like described here https://github.com/bluenviron/mediamtx/issues/2214 to have a defined behavior when the camera is not available so that there is always a stream available for the camera even if it does only show a dummy-picture when the camera is not available
                if (isCurrentlyManaged != shouldBeManaged)
                {
                    string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    if (shouldBeManaged)
                    {
                        string mediaMTXFolder = Path.Combine(location, "MediaMTX");
                        string mediaMTXExecutable = Path.Combine(mediaMTXFolder, "mediamtx");
                        if (OperatingSystem.GetCurrentOperatingSystem() is GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems.Windows)
                        {
                            mediaMTXExecutable = mediaMTXExecutable + ".exe";
                        }
                        string mediaMTXConfigurationFolder = Path.Combine(this._ApplicationConstants.GetConfigurationFolder(), "MediaMTXConfigurationFiles");
                        GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(mediaMTXConfigurationFolder);
                        string mediaMTXConfigurationFile = Path.Combine(mediaMTXConfigurationFolder, $"MediaMTXConfiguration.{camera.Id}.txt");
                        GRYLibrary.Core.Misc.Utilities.EnsureFileExists(mediaMTXConfigurationFile);
                        ushort port = this.GetNextFreePort();
                        string path = $"Stream_{camera.Id}";
                        //TODO add text-overlay-configuration into mediaMTX-Configuration to draw the current timestamp into the video, see https://github.com/bluenviron/mediamtx/pull/1604#issuecomment-1483848225

                        File.WriteAllText(mediaMTXConfigurationFile, $@"#Configuration for camera ""{camera.Name}"" (Id: {camera.Id})
rtspAddress: :{port}
paths:
  {path}:
    source: {camera.VideoInformation.StreamURL}
    sourceProtocol: tcp
");
                        using ExternalProgramExecutor process = Utilities.GetBackgroundProcess(mediaMTXExecutable, mediaMTXConfigurationFile, null, this._ApplicationConstants.GetConfigurationFolder(), null, this._Log, $"Rehost stream of camera \"{camera.Name}\" (Id: {camera.Id})", false, _ApplicationConstants.Environment);
                        this._Cameras[camera.Id] = new StreamOrganizationDataset()
                        {
                            Camera = camera,
                            Process = process,
                            Path = path,
                            Port = port,
                        };
                    }
                    else
                    {
                        var entry = this._Cameras[camera.Id];
                        if (entry.Process.IsRunning)
                        {
                            entry.Process.Terminate();
                        }
                        this._Cameras.Remove(camera.Id);
                    }
                }
            }
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
            foreach (var camera in this._Cameras)
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
                var info = this._Cameras[cameraId];
                return $"rtsp://localhost:{info.Port}/{info.Path}";
            }
        }
    }
}
