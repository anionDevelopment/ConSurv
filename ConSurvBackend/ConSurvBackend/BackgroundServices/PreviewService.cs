using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class PreviewService : IteratingBackgroundService, IPreviewService
    {
        private readonly IBusinessLogicService _CameraService;
        private readonly IRTSPManager _RTSPManager;
        private readonly IGeneralResourceLoader _GeneralResourceLoader;
        private readonly IGRYLog _Log;
        private readonly IRuntimeData _RuntimeData;
        private readonly IProcessManager _ProcessManager;
        private readonly ConcurrentDictionary<string, byte[]> _PreviewPictures;
        private readonly ITimeService _TimeService;
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly CommandlineParameter _CommandlineParameter;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        public PreviewService(IApplicationConstants<CodeUnitSpecificConstants> constants, IBusinessLogicService cameraService, IGRYLog logger, IRTSPManager rtspManager, IGRYLog gryLog, IGeneralResourceLoader generalResourceLoader, IRuntimeData runtimeData, IProcessManager processManager, ITimeService timeService, IInitializationService<CommandlineParameter> initializationService, CommandlineParameter commandlineParameter) : base(constants.ExecutionMode, logger)
        {
            this.Enabled = true;
            this._Constants = constants;
            this.AdditionalDelay = TimeSpan.FromSeconds(1);
            this._CameraService = cameraService;
            this._RTSPManager = rtspManager;
            this._Log = gryLog;
            this._GeneralResourceLoader = generalResourceLoader;
            this._PreviewPictures = new ConcurrentDictionary<string, byte[]>();
            this._RuntimeData = runtimeData;
            this._ProcessManager = processManager;
            this._TimeService = timeService;
            this._InitializationService = initializationService;
            this._CommandlineParameter = commandlineParameter;
        }

        public byte[] GetPreview(string cameraId)
        {
            lock (cameraId)
            {
                if (!this._PreviewPictures.ContainsKey(cameraId))
                {
                    this._PreviewPictures[cameraId] = this._GeneralResourceLoader.GetResource("NoPreviewAvailablePicture.jpg");
                }
                return this._PreviewPictures[cameraId];
            }
        }

        protected override void Run()
        {
            if (this._CommandlineParameter.RealRun && this._InitializationService.GetInitializationState() is Initialized)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, Model.Base.Camera> cameraKvp in this._CameraService.GetAllCameras())
                {
                    uint maximalHeightValue = 480;
                    uint maximalWidthValue = 640;
                    Model.Base.Camera camera = cameraKvp.Value;
                    try
                    {
                        string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                        if (Directory.Exists(screenshots_folder))
                        {
                            string[] files = Directory.GetFiles(screenshots_folder);
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));//wait let ffmpeg finish writing the last frame
                            System.Collections.Generic.List<string> sorted = files
                                .Where(f => f.EndsWith(".jpg"))
                                .Select(f => new FileInfo(f))
                                .OrderBy(f => f.LastWriteTimeUtc)
                                .Select(f => f.FullName)
                                .ToList();
                            if (sorted.Any())
                            {
                                string latest_preview = sorted.Last();
                                this._RuntimeData.AddPreview(camera.Id, new Model.Preview(File.ReadAllBytes(latest_preview), new FileInfo(latest_preview).LastAccessTimeUtc));
                                foreach (string file in sorted)
                                {
                                    File.Delete(file);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this._Log.Log($"Could not calculate preview for camera {cameraKvp.Key}", e);
                    }
                }
            }
        }
    }
}
