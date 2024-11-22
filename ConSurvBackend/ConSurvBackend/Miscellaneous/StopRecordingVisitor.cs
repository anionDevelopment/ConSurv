using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using ConSurvBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal class StopRecordingVisitor : IVideoTypeVisitor
    {
        private readonly IGRYLog _Log;
        private readonly Camera _Camera;
        private readonly Action<Action> _CheckAvailabilityWrapper;
        private readonly IRTSPManager _RTSPManager;
        public StopRecordingVisitor(Camera camera,IRTSPManager rtspManager, IGRYLog log)
        {
            this._RTSPManager = rtspManager;
            this._Camera = camera;
            this._Log = log;
            this._CheckAvailabilityWrapper = (action) =>
            {
                if (this._Camera.GetCurrentRecordingInformation() is Unavailable)
                {
                    this._Log.Log($"Can not stop recording of camera {this._Camera.Name} because the camera is not available.");
                }
                else
                {
                    action();
                }
            };
        }
        public void Handle(RTSPStream rtspStream)
        {
            this._CheckAvailabilityWrapper(() => rtspStream.StopRecording(this._Camera, this._RTSPManager));
        }
    }
}
