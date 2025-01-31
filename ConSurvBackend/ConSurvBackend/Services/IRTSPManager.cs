using ConSurvBackend.Core.Model.Base;
using System;

namespace ConSurvBackend.Core.Services
{
    public interface IRTSPManager
    {
        public byte[] GetPreview(Camera camera);
        public void EnsureRecordingAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC);
        public void EnsureNotRecording(Camera camera);
        public void EnsureRecordingOnMovementsAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC);
    }
}
