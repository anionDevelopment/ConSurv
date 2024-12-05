using ConSurvBackend.Core.Model.Base;
using System;

namespace ConSurvBackend.Core.Services
{
    public interface IRTSPManager
    {
        public byte[] GetPreview(string id, string streamURL);
        public void EnsureRecordingAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC);
        public void EnsureNotRecording(string cameraId);
        public void EnsureRecordingOnMovementsAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC);
    }
}
