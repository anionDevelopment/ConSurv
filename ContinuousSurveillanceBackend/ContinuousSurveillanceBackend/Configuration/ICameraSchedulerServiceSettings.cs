using GRYLibrary.Core.APIServer.Settings;
using System;

namespace ContinuousSurveillanceBackend.Core.Configuration
{
    public interface ICameraSchedulerServiceSettings : IServiceConfiguration
    {
        public TimeSpan VideoLength { get; set; }
        public string StorageFolder { get; set; }
    }
}
