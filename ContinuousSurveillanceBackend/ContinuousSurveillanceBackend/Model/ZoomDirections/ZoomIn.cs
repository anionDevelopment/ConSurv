using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.ZoomDirections
{
    public class ZoomIn : ZoomDirection
    {
        public override void Accept(IZoomDirectionVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IZoomDirectionVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
