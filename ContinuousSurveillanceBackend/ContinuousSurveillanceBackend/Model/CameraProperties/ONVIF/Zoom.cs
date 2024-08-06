using ContinuousSurveillanceBackend.Core.Model.ZoomDirections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF
{
    public class Zoom : ONVIFCommand
    {
        public ZoomDirection ZoomDirection { get; set; }

        public override void Accept(IONVIFCommandVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IONVIFCommandVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
