using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.MidT.WAF;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class CleanupService : IteratingBackgroundService, ICleanupService
    {
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly IApplicationConstants _Constants;
        private readonly IBusinessLogicService _CameraService;
        private readonly ITimeService _TimeService;
        public CleanupService(IApplicationConstants constants, IGRYLog logger, ITimeService timeService, IInitializationService<CommandlineParameter> initializationService, IBusinessLogicService cameraService) : base(constants.ExecutionMode, logger)
        {
            _Constants = constants;
            _CameraService = cameraService;
            _TimeService = timeService;
            _InitializationService = initializationService;
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(3);
        }

        protected override void Run()
        {
            if (this._InitializationService.GetInitializationState() is Initialized)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                CleanupScreenshots();
            }
            else
            {
                this._Logger.Log($"Wait until initialization is finished...", Microsoft.Extensions.Logging.LogLevel.Debug);
            }
        }

        private void CleanupScreenshots()
        {
            foreach (var camera in this._CameraService.GetAllCameras().Values)
            {
                string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                if (Directory.Exists(screenshots_folder))
                {
                    List<string> filesToDelete = Directory
                        .GetFiles(screenshots_folder)
                        .Where(file => file.EndsWith(".jpg"))
                        .Select(f => new { File = f, Timestamp = File.GetCreationTimeUtc(f) })
                        .OrderBy(dataSet => dataSet.Timestamp)
                        .Where(f => f.Timestamp < (_TimeService.GetCurrentTimeInUTCAsDateTimeOffset() - TimeSpan.FromSeconds(20)))
                        .SkipLast(2)
                        .Select(f => f.File)
                        .ToList();
                    foreach (var fileToDelete in filesToDelete)
                    {
                        GRYLibrary.Core.Misc.Utilities.EnsureFileDoesNotExist(fileToDelete);
                    }
                }
            }
        }
    }
}
