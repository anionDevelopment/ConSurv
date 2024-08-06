using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF
{
    public abstract class ONVIFCommand
    {
        public abstract void Accept(IONVIFCommandVisitor visitor);
        public abstract T Accept<T>(IONVIFCommandVisitor<T> visitor);
    }
    public interface IONVIFCommandVisitor
    {
        void Handle(Zoom zoom);
        void Handle(Move move);
    }
    public interface IONVIFCommandVisitor<T>
    {
        T Handle(Zoom zoom);
        T Handle(Move move);
    }
}
