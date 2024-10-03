namespace ConSurvBackend.Core.Model.CameraProperties.SoundTypes
{
    public abstract class SoundType
    {
        public abstract void Accept(ISoundTypeInterface visitor);
        public abstract T Accept<T>(ISoundTypeInterface<T> visitor);
    }
    public interface ISoundTypeInterface
    {
        void Handle(NoSound noSound);
        void Handle(WithSound withSound);
    }
    public interface ISoundTypeInterface<T>
    {
        T Handle(NoSound noSound);
        T Handle(WithSound withSound);
    }
}
