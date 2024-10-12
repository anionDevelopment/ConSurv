using GRYLibrary.Core.APIServer.Settings;
using System;

namespace ConSurvBackend.Core.Configuration
{
    public interface ICameraSchedulerServiceSettings : IServiceConfiguration
    {
        public TimeSpan VideoLength { get; set; }
        public string StorageFolder { get; set; }
    }
}
