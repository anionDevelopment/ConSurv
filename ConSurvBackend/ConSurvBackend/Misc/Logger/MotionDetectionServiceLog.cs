using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Misc.Logger
{
    public interface IMotionDetectionServiceLog
    {
        public IGRYLog Logger { get; }
    }
    public class MotionDetectionServiceLog : SemanticLogger, IMotionDetectionServiceLog
    {
        public IGRYLog Logger => this.Log;
        public MotionDetectionServiceLog(IGRYLogConfiguration config, string? basePath) : base(config, "MotionDetectionService", basePath)
        {
        }
    }
}
