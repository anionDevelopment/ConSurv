using ConSurvBackend.Core.Model.Base;
using System;

namespace ConSurvBackend.Core.Services
{
    public class RTSPManagerMock : IRTSPManager
    {
        public void EnsureNotRecording(string cameraId)
        {
            //TODO pretend real camera-behavior-handling
        }

        public void EnsureRecordingAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            //TODO pretend real camera-behavior-handling
        }

        public void EnsureRecordingOnMovementsAsync(Camera camera, string targetFolder, TimeSpan videoLength, bool timeInUTC)
        {
            //TODO pretend real camera-behavior-handling
        }

        public byte[] GetPreview(string id, string streamURL)
        {
            //TODO pretend real camera-behavior-handling
            throw new NotImplementedException();
        }
    }
}
