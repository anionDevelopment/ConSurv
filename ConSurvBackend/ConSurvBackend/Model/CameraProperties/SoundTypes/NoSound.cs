namespace ConSurvBackend.Core.Model.CameraProperties.SoundTypes
{
    public class NoSound : SoundType
    {
        public override void Accept(ISoundTypeInterface visitor) => visitor.Handle(this);

        public override T Accept<T>(ISoundTypeInterface<T> visitor) => visitor.Handle(this);
    }
}
