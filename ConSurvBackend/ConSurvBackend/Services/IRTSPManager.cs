using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Services
{
    public interface IRTSPManager
    {
   
        public bool IsAvailable(Camera camera);
        public (bool success, byte[] picture) GetPreview(Camera camera, uint? maximalHeight, uint? maximalWidth);
        public void EnsureRecordingAlwaysAsync(Camera camera);
        public void EnsureNotRecording(Camera camera);
        public void EnsureRecordingOnMovementsAsync(Camera camera);
    }
}
