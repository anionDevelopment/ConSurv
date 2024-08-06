using ContinuousSurveillanceBackend.Core.Model.MoveDirections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF
{
    public  class Move : ONVIFCommand
    {
        public MoveDirection MoveDirection { get; set; }

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
