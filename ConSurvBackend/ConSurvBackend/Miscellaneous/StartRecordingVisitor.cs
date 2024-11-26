using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using ConSurvBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal class StartRecordingVisitor : IVideoTypeVisitor
    {
        private readonly IGRYLog _Log;
        private readonly Camera _Camera;
        private readonly CodeUnitSpecificConfiguration _CodeUnitSpecificConfiguration;
        private readonly Action<Action> _CheckAvailabilityWrapper;
        private readonly IRTSPManager _RTSPManager;
        public StartRecordingVisitor(Camera camera, IRTSPManager rtspManager, IGRYLog log, CodeUnitSpecificConfiguration codeUnitSpecificConfiguration)
        {
            this._RTSPManager = rtspManager;
            this._Camera = camera;
            this._Log = log;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
            this._CheckAvailabilityWrapper = (action) =>
            {
                if (this._Camera.GetCurrentRecordingInformation() is Unavailable)
                {
                    this._Log.Log($"Camera {this._Camera.Name} is not available.");
                }
                else
                {
                    action();
                }
            };
        }
        public void Handle(RTSPStream rtspStream)
        {
            this._CheckAvailabilityWrapper(() => rtspStream.StartRecordingAsync(this._Camera, this._RTSPManager, this._CodeUnitSpecificConfiguration.TargetFolder, this._CodeUnitSpecificConfiguration.VideoLength, this._CodeUnitSpecificConfiguration.TimeInUTC));
        }
    }
}
