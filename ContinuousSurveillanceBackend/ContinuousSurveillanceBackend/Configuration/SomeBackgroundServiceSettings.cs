using GRYLibrary.Core.Logging.GRYLogger;

namespace ContinuousSurveillanceBackend.Core.Configuration
{
    public class SomeBackgroundServiceSettings: ISomeBackgroundServiceSettings
    {
        public bool Enabled { get; set; }
        public GRYLogConfiguration LogConfiguration { get; set; }
        public string SomePropertyOfTypeString { get; set; }
        public int SomePropertyOfTypeInt { get; set; }
        public bool SomePropertyOfTypeBool { get; set; }
    }
}
