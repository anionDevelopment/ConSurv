using ConSurvBackend.Core.Model.Base;
using System;

namespace ConSurvBackend.Core.Services
{
    [Obsolete]
    public interface IRTSPManager
    {
   
        public bool IsAvailable(Camera camera);
        public void EnsureRecordingAlwaysAsync(Camera camera);
        public void EnsureNotRecording(Camera camera);
        public void EnsureRecordingOnMovementsAsync(Camera camera);
      // public void StartStreamOfCamera(string cameraId);
    }
}
