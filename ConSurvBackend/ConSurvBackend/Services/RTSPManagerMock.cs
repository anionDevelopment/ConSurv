using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Services
{
    public class RTSPManagerMock : IRTSPManager
    {
        //TODO pretend real camera-behavior-handling
        public void EnsureNotRecording(Camera camera)
        {
            //TODO pretend real camera-behavior-handling
        }

        public void EnsureRecordingAlwaysAsync(Camera camera)
        {
            //TODO pretend real camera-behavior-handling
        }

        public void EnsureRecordingOnMovementsAsync(Camera camera)
        {
            //TODO pretend real camera-behavior-handling
        }

        public (bool success, byte[] picture) GetPreview(Camera camera, uint? maximalHeight, uint? maximalWidth,bool logFail,IGRYLog log)
        {
           throw new NotImplementedException();//TODO return (true, mock-picture)
        }

        public bool IsAvailable(Camera camera)
        {
            return true;
        }
    }
}
