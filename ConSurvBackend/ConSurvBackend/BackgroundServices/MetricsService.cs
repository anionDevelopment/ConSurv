using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using Prometheus;
using System;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class MetricsService : IteratingBackgroundService, IMetricsService
    {
        public Gauge MetricAvailableCamerasRate { get; private set; }
        private readonly IBusinessLogicService _CameraService;
        public MetricsService(IApplicationConstants<CodeUnitSpecificConstants> constants, IGRYLog logger, IBusinessLogicService cameraService) : base(constants.ExecutionMode, logger)
        {
            this.Enabled = true;
            this._CameraService = cameraService;
            this.AdditionalDelay = TimeSpan.FromMinutes(1);
            this.MetricAvailableCamerasRate = Metrics.CreateGauge(CodeUnitSpecificConstants.MetricsNameAvailableCamerasRate, "Rate of available cameras");
        }

        public void CalculateHealthAndMetrics()
        {
            try
            {
                this.MetricAvailableCamerasRate.Set(this._CameraService.GetRateOfAvailableCameras());
            }
            catch (Exception exception)
            {
                this._Logger.LogException(exception, "Error while calculating metrics");
            }
        }

        protected override void Run()
        {
            this.CalculateHealthAndMetrics();
        }
    }
}
