using GRYLibrary.Core.APIServer.BaseServices;

namespace ConSurvBackend.Core.BackgroundServices
{
    /// <summary>
    /// Marker interface for the metrics background service.
    /// Implementations periodically update Prometheus gauges with business-level camera metrics.
    /// </summary>
    public interface IMetricsService : IIteratingBackgroundService
    {
    }
}
