using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class CleanupService : IteratingBackgroundService, ICleanupService
    {
        public CleanupService(IApplicationConstants constants, IGRYLog logger) : base(constants.ExecutionMode, logger)
        {
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(2);
        }

        protected override void Run()
        {
           //TODO remove old preview-data
        }
    }
}
