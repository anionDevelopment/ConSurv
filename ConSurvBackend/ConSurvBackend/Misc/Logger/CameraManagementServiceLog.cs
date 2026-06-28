using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Misc.Logger
{
    public interface ICameraManagementServiceLog
    {
        public IGRYLog Logger { get; }
    }
    public class CameraManagementServiceLog : SemanticLogger, ICameraManagementServiceLog
    {
        public IGRYLog Logger => this.Log;
        public CameraManagementServiceLog(IGRYLogConfiguration config, string? basePath) : base(config, "CameraManagementService", basePath)
        {
        }
    }
}
