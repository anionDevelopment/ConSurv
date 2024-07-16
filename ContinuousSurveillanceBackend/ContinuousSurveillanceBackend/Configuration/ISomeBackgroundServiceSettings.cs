using GRYLibrary.Core.APIServer.Settings;

namespace ContinuousSurveillanceBackend.Core.Configuration
{
    public interface ISomeBackgroundServiceSettings : IServiceConfiguration
    {
        public string SomePropertyOfTypeString { get; set; }
        public int SomePropertyOfTypeInt { get; set; }
        public bool SomePropertyOfTypeBool { get; set; }
    }
}
