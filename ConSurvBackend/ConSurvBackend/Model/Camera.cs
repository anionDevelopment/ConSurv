using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Miscellaneous;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.RTSPStreamVideo;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Model
{
    public class Camera
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsONVIFCamera { get; internal set; }
        public VideoType VideoType { get; internal set; }
        public RecordMode RecordingMode { get; internal set; }

        public Camera(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.IsONVIFCamera = false;
            this.VideoType = new RTSPStream();
            this.RecordingMode = new NoRecording();

        }

        public bool IsAvailable()
        {
            return this.GetCurrentRecordingInformation() is not Unavailable;
        }

        public RecordingState GetCurrentRecordingInformation()
        {
            return new Idle();//TODO
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

        internal CameraDTO ToDTO()
        {
            return new CameraDTO()
            {
                CameraId = this.Id,
                Name = this.Name,
                RecordMode = this.RecordingMode.ToDTO(),
                VideoType = this.VideoType.ToDTO(),
                CurrentRecordingState = this.GetCurrentRecordingInformation().ToDTO(),
            };
        }
    }
}
