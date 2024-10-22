using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Configuration
{
    public class CameraSchedulerServiceSettings : ICameraSchedulerServiceSettings
    {
        public bool Enabled { get; set; }
        public GRYLogConfiguration LogConfiguration { get; set; }
        public TimeSpan VideoLength { get; set; }
        public string StorageFolder { get; set; }
    }
}
