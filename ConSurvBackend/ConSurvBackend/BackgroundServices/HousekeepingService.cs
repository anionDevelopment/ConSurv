using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Misc.Logger;
using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class HousekeepingService : IteratingBackgroundService, IHousekeepingService
    {
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly IApplicationConstants _Constants;
        private readonly IBusinessLogicService _CameraService;
        private readonly ITimeService _TimeService;
        private readonly IRuntimeData _RuntimeData;
        private readonly IAuditLog _AuditLog;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _Configuration;
        private readonly IDictionary<string/*camera-id*/, DateTime> _LastUsedScreenshotsForMotionDetection = new Dictionary<string, DateTime>();
        /// <summary>
        /// Initializes a new instance of <see cref="HousekeepingService"/> with all required dependencies.
        /// </summary>
        /// <param name="constants">Application-wide constants, including the data-folder path.</param>
        /// <param name="logger">Logger used by the base class and this service.</param>
        /// <param name="timeService">Service used to obtain the current date/time.</param>
        /// <param name="initializationService">Service that tracks the application initialization state.</param>
        /// <param name="cameraService">Service used to retrieve camera data.</param>
        /// <param name="runtimeData">Shared in-memory runtime state (previews, fallback picture, etc.).</param>
        /// <param name="config">Persisted configuration containing retention and motion-detection settings.</param>
        /// <param name="auditLog">Audit log used to record detected motion events.</param>
        public HousekeepingService(IApplicationConstants constants, IHousekeepingServiceLog logger, ITimeService timeService, IInitializationService<CommandlineParameter> initializationService, IBusinessLogicService cameraService, IRuntimeData runtimeData, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> config, IAuditLog auditLog) : base(constants.ExecutionMode, logger.Logger)
        {
            this._Constants = constants;
            this._CameraService = cameraService;
            this._Configuration = config;
            this._AuditLog = auditLog;
            this._TimeService = timeService;
            this._InitializationService = initializationService;
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(1);
            this._RuntimeData = runtimeData;
        }

        /// <inheritdoc />
        protected override void Run()
        {
            if (this._InitializationService.GetInitializationState() is Initialized)
            {
                LogLevel logLevelForOverhead = LogLevel.Debug;
                this._Logger.RunTask(this.UpdatePreviewsInRuntimeData, nameof(UpdatePreviewsInRuntimeData), true, logLevelForOverhead);
                this._Logger.RunTask(this.DoMotionDetection, nameof(DoMotionDetection), true, logLevelForOverhead);
                this._Logger.RunTask(this.CleanupScreenshots, nameof(CleanupScreenshots), true, logLevelForOverhead);
                this._Logger.RunTask(this.CleanupRecordings, nameof(CleanupRecordings), true, logLevelForOverhead);
            }
            else
            {
                this._Logger.Log($"{this.GetType().Name}: Wait until initialization is finished...", Microsoft.Extensions.Logging.LogLevel.Debug);
            }
        }

        private void DoMotionDetection()
        {
            return;//TODO re-activate motion detection problem: open-cv is currently not loadable on linux
            foreach (Camera camera in this._CameraService.GetAllCameras().Values)
            {
                string screenshotsFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                List<string> availableScreenshots;//TODO get the available screenshots from _RuntimeData instead
                if (Directory.Exists(screenshotsFolder))
                {
                    DateTimeOffset now = this._TimeService.GetCurrentTimeInUTCAsDateTimeOffset();
                    availableScreenshots = Directory
                         .GetFiles(screenshotsFolder)
                         .Where(file => file.EndsWith(".jpg"))
                         .Select(f => new { File = f, Timestamp = File.GetCreationTimeUtc(f) })
                         .Where(f => (now - TimeSpan.FromSeconds(20)) < f.Timestamp)
                         .OrderBy(dataSet => dataSet.Timestamp)
                         .Where(f =>
                         {
                             if (this._LastUsedScreenshotsForMotionDetection.TryGetValue(camera.Id, out DateTime value))
                             {
                                 return value < f.Timestamp;
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
                        List<string> last2Screenshots = availableScreenshots[^2..];
                        string screenshot1 = last2Screenshots[0];
                        string screenshot2 = last2Screenshots[1];
                        DateTime creationDate1 = File.GetCreationTimeUtc(screenshot1);
                        this._LastUsedScreenshotsForMotionDetection[camera.Id] = creationDate1;
                        if (this.ImagesAreDifferent(screenshot1, screenshot2))
                        {
                            this._AuditLog.Logger.Log($"Motion detected for camera '{camera.Name}'. (Camera-id: {camera.Id})");
                            //TODO raise event
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether two screenshot files represent different frames by comparing their
        /// perceptual hash similarity against the configured motion-detection threshold.
        /// </summary>
        /// <param name="screenshot1">Absolute path of the first (older) screenshot file.</param>
        /// <param name="screenshot2">Absolute path of the second (newer) screenshot file.</param>
        /// <returns>
        /// <see langword="true"/> if the similarity score is below the configured
        /// <see cref="CodeUnitSpecificConfiguration.MotionDetectionThreshold"/>; otherwise
        /// <see langword="false"/>.
        /// </returns>
        private bool ImagesAreDifferent(string screenshot1, string screenshot2)
        {
            return Misc.Utilities.CalculateImageSimilarity(File.ReadAllBytes(screenshot1), File.ReadAllBytes(screenshot2)) < this._Configuration.ApplicationSpecificConfiguration.MotionDetectionThreshold;
        }


        /// <summary>
        /// Reads the most recent screenshot for every configured camera and stores it as the
        /// current preview in the shared runtime data.  If no screenshot exists for a camera,
        /// the fallback picture is used instead.
        /// </summary>
        private void UpdatePreviewsInRuntimeData()
        {
            foreach (Camera camera in this._CameraService.GetAllCameras().Values)
            {
                string screenshotsFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                List<string> availableScreenshots;
                if (Directory.Exists(screenshotsFolder))
                {
                    availableScreenshots = Directory
                         .GetFiles(screenshotsFolder)
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
                    preview = this._RuntimeData.GetPreviewFallbackPicture();
                }
                else
                {
                    string screenshot = availableScreenshots.Last();
                    preview = File.ReadAllBytes(screenshot);
                }
                this._RuntimeData.AddPreview(camera.Id, new Preview(preview, this._TimeService.GetCurrentLocalTimeAsDateTimeOffset()));
            }
        }

        /// <summary>
        /// Deletes outdated screenshot files for all cameras, keeping only screenshots that are
        /// newer than 20 seconds or are among the two most recent files.
        /// </summary>
        private void CleanupScreenshots()
        {
            foreach (Camera camera in this._CameraService.GetAllCameras().Values)
            {
                string screenshotsFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Screenshots");
                if (Directory.Exists(screenshotsFolder))
                {
                    DateTimeOffset now = this._TimeService.GetCurrentTimeInUTCAsDateTimeOffset();
                    List<string> filesToDelete = Directory
                        .GetFiles(screenshotsFolder)
                        .Where(file => file.EndsWith(".jpg"))
                        .Select(f => new { File = f, Timestamp = File.GetCreationTimeUtc(f) })
                        .Where(f => f.Timestamp < (now - TimeSpan.FromSeconds(20)))
                        .OrderBy(dataSet => dataSet.Timestamp)
                        .SkipLast(2)
                        .Select(f => f.File)
                        .ToList();
                    foreach (string fileToDelete in filesToDelete)
                    {
                        GRYLibrary.Core.Misc.Utilities.EnsureFileDoesNotExist(fileToDelete);
                    }
                }
            }
        }
        /// <summary>
        /// Deletes recording files that have exceeded the configured video retention period
        /// (<see cref="CodeUnitSpecificConfiguration.VideoRetentionPeriod"/>) for all cameras.
        /// </summary>
        private void CleanupRecordings()
        {
            foreach (Camera camera in this._CameraService.GetAllCameras().Values)
            {
                string recordingsFolder = Path.Combine(this._Constants.GetDataFolder(), "CameraData", camera.Id, "Recordings");
                if (Directory.Exists(recordingsFolder))
                {
                    DateTimeOffset now = this._TimeService.GetCurrentTimeInUTCAsDateTimeOffset();
                    List<string> filesToDelete = Directory
                        .GetFiles(recordingsFolder)
                        .Where(file => file.EndsWith(".mp4"))
                        .Select(f => new { File = f, Timestamp = File.GetCreationTimeUtc(f) })
                        .Where(f => f.Timestamp < (now - this._Configuration.ApplicationSpecificConfiguration.VideoRetentionPeriod))
                        .Select(f => f.File)
                        .ToList();
                    foreach (string fileToDelete in filesToDelete)
                    {
                        GRYLibrary.Core.Misc.Utilities.EnsureFileDoesNotExist(fileToDelete);
                    }
                }
            }
        }
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //add dispose logic here if required
            }
            base.Dispose(disposing);
        }
    }
}
