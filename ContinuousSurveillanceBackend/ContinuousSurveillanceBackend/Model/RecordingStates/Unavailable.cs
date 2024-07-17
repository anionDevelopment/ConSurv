using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.RecordingStates
{
    public class Unavailable : RecordingState
    {
        public override T Accept<T>(IRecordingStateVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public override void Accept(IRecordingStateVisitor visitor)
        {
            visitor.Handle(this);
        }
    }
}
