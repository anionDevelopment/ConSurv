using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Core.Services
{
    public interface IRTSPManager
    {
   
        public bool IsAvailable(Camera camera);
        public (bool success, byte[] picture) GetPreviewDirectlyFromCamera(Camera camera, uint? maximalHeight, uint? maximalWidth, bool logFail, IGRYLog log);
        public void EnsureRecordingAlwaysAsync(Camera camera);
        public void EnsureNotRecording(Camera camera);
        public void EnsureRecordingOnMovementsAsync(Camera camera);
        public string StartStreamOfCamera(string cameraId);
    }
}
