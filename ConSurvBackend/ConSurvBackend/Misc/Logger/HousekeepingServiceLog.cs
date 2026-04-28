using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Misc.Logger
{
    public interface IHousekeepingServiceLog
    {
        public IGRYLog Logger { get; }
    }
    public class HousekeepingServiceLog : SemanticLogger, IHousekeepingServiceLog
    {
        public IGRYLog Logger => this.Log;
        public HousekeepingServiceLog(IGRYLogConfiguration config, string? basePath) : base(config, "HousekeepingService", basePath)
        {
        }
    }
}
