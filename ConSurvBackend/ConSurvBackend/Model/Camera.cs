using ConSurvBackend.Core.Model.CameraProperties.SoundTypes;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes;
using ConSurvBackend.Core.Model.RecordingModes;
using ConSurvBackend.Core.Model.RecordingStates;
using System;

namespace ConSurvBackend.Core.Model
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public VideoType VideoType { get; set; }
        public SoundType SoundType { get; set; }
        public RecordMode RecordingMode { get; set; }

        public bool IsAvailable()
        {
            return this.GetCurrentRecordingInformation() is not Unavailable;
        }

        public RecordingState GetCurrentRecordingInformation()
        {
            throw new NotImplementedException();
        }
    }
}
