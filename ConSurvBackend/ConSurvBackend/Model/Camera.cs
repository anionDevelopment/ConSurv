using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Miscellaneous;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using ConSurvBackend.Core.Model.RecordingModes;
using ConSurvBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Model
{
    public class Camera
    {
        private readonly IGRYLog _Log;
        private readonly IRTSPManager _RTSPManager;
        private readonly CodeUnitSpecificConfiguration _CodeUnitSpecificConfiguration;
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsONVIFCamera { get; internal set; }
        private VideoType _VideoType;
        public VideoType VideoType
        {
            get
            {
                return this._VideoType;
            }
            internal set
            {
                RecordMode recordMode = this.RecordingMode;
                this.RecordingMode = new NoRecording();
                this._VideoType = value;
                this.RecordingMode = recordMode;
            }
        }
        private RecordMode _RecordingMode;
        public RecordMode RecordingMode
        {
            get
            {
                return this._RecordingMode;
            }
            internal set
            {
                if (this._RecordingMode != value)
                {
                    value.Accept(new ChangeRecordingModeVisitor(this, this._RTSPManager, this._Log, this._CodeUnitSpecificConfiguration));
                    this._RecordingMode = value;
                }

            }
        }

        public Camera(string id, string name, IGRYLog log, IRTSPManager rtspManager, CodeUnitSpecificConfiguration codeUnitSpecificConfiguration)
        {
            this.Id = id;
            this.Name = name;
            this.VideoType = null;
            this.IsONVIFCamera = false;
            this.RecordingMode = new NoRecording();
            this._RTSPManager = rtspManager;
            this._Log = log;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;

        }

        public bool IsAvailable()
        {
            return this.GetCurrentRecordingInformation() is not Unavailable;
        }

        public RecordingState GetCurrentRecordingInformation()
        {
            throw new NotImplementedException();
        }
        internal void EnsureIsRecording()
        {
            if (this.RecordingMode is not RecordAlways)
            {
                this.RecordingMode = new RecordAlways();
            }
        }
        internal void EnsureIsNotRecording()
        {
            if (this.RecordingMode is not RecordAlways)
            {
                this.RecordingMode = new NoRecording();
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Camera camera &&
                   this.Id == camera.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }
    }
}
