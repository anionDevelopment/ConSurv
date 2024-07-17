using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.RecordingStates
{
    public abstract class RecordingState
    {
        public abstract T Accept<T>(IRecordingStateVisitor<T> visitor);
        public abstract void Accept(IRecordingStateVisitor visitor);
    }
    public interface IRecordingStateVisitor<T>
    {
        T Handle(CurrentlyRecording currentlyRecording);
        T Handle(Idle idle);
        T Handle(Unavailable unavailable);
    }
    public interface IRecordingStateVisitor
    {
        void Handle(CurrentlyRecording currentlyRecording);
        void Handle(Idle idle);
        void Handle(Unavailable unavailable);
    }
}
