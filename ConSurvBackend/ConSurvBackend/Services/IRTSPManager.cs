using System;

namespace ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo
{
    public interface IRTSPManager
    {
        byte[] GetPreview(string id, string streamURL);
        public void StartRecordingAsync(string cameraId, string streamURL, string targetFolder, TimeSpan videoLength, bool timeInUTC);
        internal void StopRecording(string cameraId);
    }
}
