using System;
using System.Diagnostics;

namespace ConSurvBackend.Core.Model.RecordingStates
{
    public class CurrentlyRecording : RecordingState
    {
        public override T Accept<T>(IRecordingStateVisitor<T> visitor) => visitor.Handle(this);

        public override void Accept(IRecordingStateVisitor visitor) => visitor.Handle(this);
        public Process GetRecordingProcess() => throw new NotImplementedException();
    }
}
