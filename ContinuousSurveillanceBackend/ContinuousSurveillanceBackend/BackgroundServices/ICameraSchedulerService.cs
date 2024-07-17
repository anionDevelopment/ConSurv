using ContinuousSurveillanceBackend.Core.Model;
using GRYLibrary.Core.APIServer.BaseServices;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace ContinuousSurveillanceBackend.Core.BackgroundServices
{
    public interface ICameraSchedulerService: IIteratingBackgroundService
    {
        public ObservableGauge<decimal> AvailableCamerasRatio { get; }
        decimal CalculateAvailableCamerasRatio();
        ISet<Camera> GetAllCameras();
    }
}
