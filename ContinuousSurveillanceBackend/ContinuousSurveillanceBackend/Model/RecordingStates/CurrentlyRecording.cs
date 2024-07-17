using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.RecordingStates
{
    public class CurrentlyRecording : RecordingState
    {
        public override T Accept<T>(IRecordingStateVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }

        public override void Accept(IRecordingStateVisitor visitor)
        {
             visitor.Handle(this);
        }
        public Process GetRecordingProcess()
        {
            throw new NotImplementedException();
        }
    }
}
