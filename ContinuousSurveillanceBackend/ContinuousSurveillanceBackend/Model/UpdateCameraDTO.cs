namespace ContinuousSurveillanceBackend.Core.Model
{
    public class UpdateCameraDTO
    {
        public string Name { get; set; }
        public string CameraAddress { get; set; }
        public RecordMode RecordMode { get; set; }
    }
}
