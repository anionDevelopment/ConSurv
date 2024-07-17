namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.SoundTypes
{
    public class NoSound : SoundType
    {
        public override void Accept(ISoundTypeInterface visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ISoundTypeInterface<T> visitor)
        {
          return visitor.Handle(this);
        }
    }
}
