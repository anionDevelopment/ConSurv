using GRYLibrary.Core.APIServer.BaseServices;

namespace ConSurvBackend.Core.BackgroundServices
{
    /// <summary>
    /// Marker interface for the camera management background service.
    /// Implementations are responsible for keeping camera stream processes (MediaMTX, FFMPEG)
    /// in the desired state for every configured camera.
    /// </summary>
    public interface ICameraManagementService : IIteratingBackgroundService
    {
    }
}
