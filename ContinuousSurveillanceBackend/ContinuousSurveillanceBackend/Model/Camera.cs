using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using ContinuousSurveillanceBackend.Core.Model.RecordingStates;
using System;

namespace ContinuousSurveillanceBackend.Core.Model
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
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
