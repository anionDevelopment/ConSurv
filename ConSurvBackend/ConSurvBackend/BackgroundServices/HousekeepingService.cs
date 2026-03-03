using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
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
    public class HousekeepingService : IteratingBackgroundService, IHousekeepingService
    {
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly IApplicationConstants _Constants;
        private readonly IBusinessLogicService _CameraService;
        private readonly ITimeService _TimeService;
        private readonly IRuntimeData _RuntimeData;
        private readonly IDictionary<string/*camera-id*/, DateTime> _LastUsedScreenshotsForMotionDetection = new Dictionary<string, DateTime>();
        public HousekeepingService(IApplicationConstants constants, IGRYLog logger, ITimeService timeService, IInitializationService<CommandlineParameter> initializationService, IBusinessLogicService cameraService, IRuntimeData runtimeData) : base(constants.ExecutionMode, logger)
        {
            _Constants = constants;
            _CameraService = cameraService;
            _TimeService = timeService;
            _InitializationService = initializationService;
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(3);
            this._RuntimeData = runtimeData;
        }

        protected override void Run()
        {
            if (this._InitializationService.GetInitializationState() is Initialized)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                UpdatePreviewsInRuntimeData();
                DoMotionDetection();
                CleanupScreenshots();
            }
            else
            {
                this._Logger.Log($"Wait until initialization is finished...", Microsoft.Extensions.Logging.LogLevel.Debug);
            }
        }

        private void DoMotionDetection()
        {
            foreach (var camera in this._CameraService.GetAllCameras().Values)
            {
                string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                List<string> availableScreenshots;
                if (Directory.Exists(screenshots_folder))
                {
                    availableScreenshots = Directory
                         .GetFiles(screenshots_folder)
                         .Where(file => file.EndsWith(".jpg"))
                         .Select(f => new { File = f, Timestamp = File.GetCreationTimeUtc(f) })
                         .OrderBy(dataSet => dataSet.Timestamp)
                         .Where(f => (_TimeService.GetCurrentTimeInUTCAsDateTimeOffset() - TimeSpan.FromSeconds(20)) < f.Timestamp)
                         .Where(f =>
                         {
                             if (_LastUsedScreenshotsForMotionDetection.ContainsKey(camera.Id))
                             {
                                 return _LastUsedScreenshotsForMotionDetection[camera.Id] < f.Timestamp;
                             }
                             else
                             {
                                 return true;
                             }
                         })
                         .Select(f => f.File)
                         .ToList();
                    if (availableScreenshots.Count > 1)
                    {
                        var last2Screenshots = availableScreenshots[^2..];
                        var screenshot1 = last2Screenshots[0];
                        var screenshot2 = last2Screenshots[1];
                        var creationDate1 = File.GetCreationTimeUtc(screenshot1);
                        _LastUsedScreenshotsForMotionDetection[camera.Id] = creationDate1;
                        if (ImagesAreDifferent(screenshot1, screenshot2))
                        {
                            //TODO raise event
                        }
                    }
                }
            }
        }

        private bool ImagesAreDifferent(string screenshot1, string screenshot2)
        {
            //TODO use configurable threshold
            return false;//TODO implement
        }

        private void UpdatePreviewsInRuntimeData()
        {
            foreach (var camera in this._CameraService.GetAllCameras().Values)
            {
                string screenshots_folder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                List<string> availableScreenshots;
                if (Directory.Exists(screenshots_folder))
                {
                    availableScreenshots = Directory
                         .GetFiles(screenshots_folder)
                         .Where(file => file.EndsWith(".jpg"))
                         .Select(f => new { File = f, Timestamp = File.GetCreationTimeUtc(f) })
                         .OrderBy(dataSet => dataSet.Timestamp)
                         .Select(f => f.File)
                         .ToList();
                }
                else
                {
                    availableScreenshots = new List<string>();
                }
                byte[] preview;
                if (availableScreenshots.Count == 0)
                {
                    preview = _RuntimeData.GetPreviewFallbackPicture();
                }
                else
                {
                    string screenshot = availableScreenshots.Last();
                    preview = File.ReadAllBytes(screenshot);
                }
                _RuntimeData.AddPreview(camera.Id, new Preview(preview, this._TimeService.GetCurrentLocalTimeAsDateTimeOffset()));
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
