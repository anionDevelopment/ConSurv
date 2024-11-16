using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GRYLogger;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class MetricsService : IteratingBackgroundService, IMetricsService
    {
        public Gauge MetricAvailableCamerasRate { get; private set; }
        private readonly ICameraService _CameraService;
        public MetricsService(IApplicationConstants<CodeUnitSpecificConstants> constants, IGRYLog logger, ICameraService cameraService) : base(constants.ExecutionMode, logger)
        {
            this.Enabled = true;
            this._CameraService = cameraService;
            this.AdditionalDelay = TimeSpan.FromMinutes(1);
            this.MetricAvailableCamerasRate = Metrics.CreateGauge(CodeUnitSpecificConstants.MetricsNameAvailableCamerasRate, "Amount of existing datasets");
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
