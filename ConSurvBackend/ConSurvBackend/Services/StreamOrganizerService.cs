using ConSurvBackend.Core.Miscellaneous;
using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.OperatingSystem;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ConSurvBackend.Core.Services
{
    public class StreamOrganizerService : IStreamOrganizerService
    {
        private readonly ICameraService _CameraService;
        private static readonly object _Lock = new object();
        private readonly IDictionary<string, StreamOrganizationDataset> _Cameras;
        private readonly IApplicationConstants _ApplicationConstants;
        public StreamOrganizerService(ICameraService cameraService, IApplicationConstants applicationConstants)
        {
            _CameraService = cameraService;
            _ApplicationConstants = applicationConstants;
            _Cameras = new Dictionary<string, StreamOrganizationDataset>();
        }
        public void OrganizeCamera(Camera camera)
        {
            lock (_Lock)
            {
                bool isCurrentlyManaged = _Cameras.ContainsKey(camera.Id);
                bool shouldBeManaged = camera.Enabled && _CameraService.IsAvailable(camera);
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
                        string mediaMTXConfigurationFolder = Path.Combine(_ApplicationConstants.GetConfigurationFolder(), "MediaMTXConfigurationFiles");
                        GRYLibrary.Core.Misc.Utilities.EnsureDirectoryExists(mediaMTXConfigurationFolder);
                        string mediaMTXConfigurationFile = Path.Combine(mediaMTXConfigurationFolder, $"MediaMTXConfiguration.{camera.Id}.txt");
                        GRYLibrary.Core.Misc.Utilities.EnsureFileExists(mediaMTXConfigurationFile);
                        ushort port = GetNextFreePort();
                        string path = $"Stream_{camera.Id}";
                        //TODO add text-overlay-configuration into mediaMTX-Configuration to draw the current timestamp into the video, see https://github.com/bluenviron/mediamtx/pull/1604#issuecomment-1483848225
                        File.WriteAllText(mediaMTXConfigurationFile, $@"#Configuration for camera ""{camera.Name}"" (Id: {camera.Id})
rtsp:
  enabled: yes
  address: :{port}

paths:
  {path}:
    source: {camera.VideoInformation.StreamURL}
    sourceProtocol: tcp
");
                        var process = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
                        {
                            Program = mediaMTXExecutable,
                            Argument = mediaMTXConfigurationFile,
                        });
                        process.Configuration.WaitingState = new RunAsynchronously();
                        process.Run();
                        _Cameras[camera.Id] = new StreamOrganizationDataset()
                        {
                            Camera = camera,
                            Process = process,
                            Path = path,
                            Port = port,
                        };
                    }
                    else
                    {
                        var entry = _Cameras[camera.Id];
                        if (entry.Process.IsRunning)
                        {
                            entry.Process.Terminate();
                        }
                        _Cameras.Remove(camera.Id);
                    }
                }
            }
        }

        private ushort GetNextFreePort()
        {
            ushort port = 10000;
            while (port < ushort.MaxValue)
            {
                if (PortIsFree(port))
                {
                    return port;
                }
                port = (ushort)(port + 1);
            }
            throw new InternalAlgorithmException($"Can not find a free port.");
        }

        private bool PortIsFree(ushort port)
        {
            foreach (var camera in _Cameras)
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
                return _Cameras.ContainsKey(cameraId);
            }
        }

        public string GetStreamURL(string cameraId)
        {
            lock (_Lock)
            {
                var info = _Cameras[cameraId];
                return $"rtsp://localhost:{info.Port}/{info.Path}";
            }
        }
    }
}
