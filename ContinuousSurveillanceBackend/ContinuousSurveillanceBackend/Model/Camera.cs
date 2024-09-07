using ContinuousSurveillanceBackend.Core.Model.CameraProperties.SoundTypes;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using ContinuousSurveillanceBackend.Core.Model.RecordingStates;
using System;

namespace ContinuousSurveillanceBackend.Core.Model
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public VideoType VideoType { get; set; }
        public SoundType SoundType { get; set; }
        public RecordMode RecordingMode { get; set; }

        public bool IsAvailable() => this.GetCurrentRecordingInformation() is not Unavailable;
        public RecordingState GetCurrentRecordingInformation() => throw new NotImplementedException();
    }
}
