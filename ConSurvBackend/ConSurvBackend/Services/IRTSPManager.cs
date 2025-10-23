using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Services
{
    public interface IRTSPManager
    {
   
        public bool IsAvailable(Camera camera);
        public void EnsureRecordingAlwaysAsync(Camera camera);
        public void EnsureNotRecording(Camera camera);
        public void EnsureRecordingOnMovementsAsync(Camera camera);
      // public void StartStreamOfCamera(string cameraId);
    }
}
