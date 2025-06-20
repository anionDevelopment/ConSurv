using ConSurvBackend.Core.Model.RecordModes;
using System;

namespace ConSurvBackend.Core.Model.Base
{
    public class Camera
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public VideoInformation VideoInformation { get; internal set; }
        public RecordMode RecordMode { get; internal set; }
        public bool Enabled { get; internal set; }

        public Camera(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.VideoInformation = new VideoInformation()
            {
                Certificate=null,
                IsONVIFCamera=false,
            };
            this.RecordMode = new NoRecording();
            this.Enabled = true;
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
