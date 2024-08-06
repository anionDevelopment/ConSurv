using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.MoveDirections
{
    public class MoveLeft : MoveDirection
    {
        public override void Accept(IMoveDirectionVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IMoveDirectionVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
