using GRYLibrary.Core.APIServer.BaseServices;

namespace ConSurvBackend.Core.BackgroundServices
{
    /// <summary>
    /// Marker interface for the housekeeping background service.
    /// Implementations are responsible for periodically updating camera previews, running motion
    /// detection, and deleting outdated screenshot and recording files.
    /// </summary>
    public interface IHousekeepingService:IIteratingBackgroundService
    {
    }
}
