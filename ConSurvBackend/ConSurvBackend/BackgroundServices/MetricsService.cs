using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Misc.Logger;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Settings;
using Prometheus;
using System;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class MetricsService : IteratingBackgroundService, IMetricsService
    {
        public Gauge MetricAvailableCamerasRate { get; private set; }
        private readonly IBusinessLogicService _CameraService;
        /// <summary>
        /// Initializes a new instance of <see cref="MetricsService"/> and registers the Prometheus
        /// gauge for the available-cameras rate.
        /// </summary>
        /// <param name="constants">Application-wide constants used to retrieve the execution mode.</param>
        /// <param name="logger">Logger passed to the base class.</param>
        /// <param name="cameraService">Service used to query the rate of available cameras.</param>
        public MetricsService(IApplicationConstants<CodeUnitSpecificConstants> constants, IMetricsServiceLog logger, IBusinessLogicService cameraService) : base(constants.ExecutionMode, logger.Logger)
        {
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromMinutes(1);
            this._CameraService = cameraService;
            this.MetricAvailableCamerasRate = Metrics.CreateGauge(CodeUnitSpecificConstants.MetricsNameAvailableCamerasRate, "Rate of available cameras");
        }

        /// <summary>
        /// Queries the current rate of available cameras and updates the
        /// <see cref="MetricAvailableCamerasRate"/> Prometheus gauge.
        /// Any exception raised during the calculation is caught and logged without propagating.
        /// </summary>
        public void CalculateHealthAndMetrics()
        {
            try
            {
                this.MetricAvailableCamerasRate.Set(this._CameraService.GetRateOfAvailableCameras());
            }
            catch (Exception exception)
            {
                this._Logger.Log( "Error while calculating metrics",exception);
            }
        }

        /// <inheritdoc />
        protected override void Run()
        {
            this.CalculateHealthAndMetrics();
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
