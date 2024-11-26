using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using System;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class RecordModeDTO
    {
        public string RecordMode { get; set; }

        public RecordMode ToRecordMode()
        {
            return this.RecordMode switch
            {
                nameof(NoRecording) => this.LoadNoRecording(),
                nameof(RecordAlways) => this.LoadRecordAlways(),
                _ => throw new NotSupportedException($"Unsupported {nameof(this.RecordMode)}: '{this.RecordMode}'"),
            };
        }

        private NoRecording LoadNoRecording()
        {
            NoRecording result = new NoRecording();
            //TODO set properties
            return result;
        }

        private RecordAlways LoadRecordAlways()
        {
            RecordAlways result = new RecordAlways();
            //TODO set properties
            return result;
        }
    }
}
