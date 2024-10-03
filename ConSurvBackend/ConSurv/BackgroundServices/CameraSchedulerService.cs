using ConSurvBackend.Core.Configuration;
using GRYLibrary.Core.APIServer.Settings;
using System.Threading;
using System;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.Misc;
using System.Diagnostics.Metrics;
using ConSurvBackend.Core.Constants;
using System.Collections.Generic;
using System.Linq;
using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Miscellaneous;
using GRYLibrary.Core.APIServer.Settings.Configuration;

namespace ConSurvBackend.Core.BackgroundServices
{
    public sealed class CameraSchedulerService : IteratingBackgroundService, ICameraSchedulerService
    {

        private readonly Meter _AvailableCamerasRatioMeter;
        private readonly IList<Camera> _Cameras;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _CodeUnitSpecificConfiguration;
        private readonly ICameraSchedulerServiceSettings _CameraSchedulerServiceSettings;
        public ObservableGauge<decimal> AvailableCamerasRatio { get; private set; }

        public CameraSchedulerService(ICameraSchedulerServiceSettings cameraSchedulerServiceSettings, IApplicationConstants applicationConstants, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> codeUnitSpecificConfiguration) : base(applicationConstants.ExecutionMode, GeneralLoggerExtensions.SetupLogger(cameraSchedulerServiceSettings.LogConfiguration, applicationConstants.GetLogFolder(), nameof(CameraSchedulerService)))
        {
            this._CameraSchedulerServiceSettings = cameraSchedulerServiceSettings;
            this.Enabled = this._CameraSchedulerServiceSettings.Enabled;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;

            this._AvailableCamerasRatioMeter = new Meter(CodeUnitSpecificConstants.AvailableCamerasRatioMeterName);//https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation
            this.AvailableCamerasRatio = this._AvailableCamerasRatioMeter.CreateObservableGauge<decimal>(CodeUnitSpecificConstants.AvailableCamerasRatioMeterName, this.CalculateAvailableCamerasRatio);
            this._Cameras = new List<Camera>();
        }

        protected override void Run()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            this.CameraManagementLoop();
            this.CalculateAvailableCamerasRatio();
        }

        private void CameraManagementLoop()
        {
            foreach (var camera in this._Cameras)
            {
                try
                {
                    this.CameraManagementLoop(camera);
                }
                catch (Exception ex)
                {
                    this._Logger.LogException(ex, $"Exception while management-loop for camera ${camera.Id}.");
                }
            }
        }

        private void CameraManagementLoop(Camera camera) => camera.RecordingMode.Accept(new CameraManagementLoopVisitor(this._Logger, camera, this._CodeUnitSpecificConfiguration.ApplicationSpecificConfiguration));
        public decimal CalculateAvailableCamerasRatio()
        {
            if (this._Cameras.Count == 0)
            {
                return 1;
            }
            else
            {
                return Math.Round(this._Cameras.Count(camera => camera.IsAvailable()) / (decimal)this._Cameras.Count, 2);
            }
        }
        public override void Dispose() => this._AvailableCamerasRatioMeter.Dispose();

        public ISet<Camera> GetAllCameras() => this._Cameras.ToHashSet();
    }
}
