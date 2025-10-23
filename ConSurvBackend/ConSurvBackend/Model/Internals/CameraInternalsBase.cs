using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.Internals
{
    public abstract class CameraInternalsBase
    {
        public  Camera Camera { get;private set; }
        public CameraInternalsBase(Camera camera)
        {
            this.Camera = camera;
        }
        public abstract void Accept(ICameraInternalsBaseVisitor visitor);
        public abstract T Accept<T>(ICameraInternalsBaseVisitor<T> visitor);
    }
    public interface ICameraInternalsBaseVisitor
    {
        void Handle(Available available);
        void Handle(NotAvailable notAvailable);
    }
    public interface ICameraInternalsBaseVisitor<T>
    {
        T Handle(Available available);
        T Handle(NotAvailable notAvailable);
    }
}
