using GRYLibrary.Core.Logging.GRYLogger;

namespace ContinuousSurveillanceBackend.Core.Configuration
{
    public class CameraSchedulerServiceSettings: ICameraSchedulerServiceSettings
    {
        public bool Enabled { get; set; }
        public GRYLogConfiguration LogConfiguration { get; set; }
    }
}
