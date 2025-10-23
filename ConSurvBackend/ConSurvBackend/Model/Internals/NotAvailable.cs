using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Model.Internals
{
    public class NotAvailable : CameraInternalsBase
    {
        public NotAvailable(Camera camera) : base(camera)
        {
        }
        public override void Accept(ICameraInternalsBaseVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ICameraInternalsBaseVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
