using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.RecordStates;
using System;

namespace ConSurvBackend.Core.Model.Base
{
    public class Camera
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public VideoInformation VideoInformation { get; internal set; }
        public RecordMode RecordMode { get; internal set; }

        public Camera(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.VideoInformation = new VideoInformation();
            this.RecordMode = new NoRecording();
        }

        public bool IsAvailable()
        {
            return this.GetCurrentRecordingInformation() is not Unavailable;
        }

        public RecordState GetCurrentRecordingInformation()
        {
            return new Idle();//TODO
        }
        internal void EnsureIsRecording()
        {
            if (this.RecordMode is not RecordAlways)
            {
                this.RecordMode = new RecordAlways();
            }
        }
        internal void EnsureIsNotRecording()
        {
            if (this.RecordMode is not RecordAlways)
            {
                this.RecordMode = new NoRecording();
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
                RecordMode = this.RecordMode.ToDTO(),
                VideoInformation = this.VideoInformation.ToDTO(),
                RecordState = this.GetCurrentRecordingInformation().ToDTO(),
            };
        }
    }
}
