using ConSurvBackend.Core.Model.RecordModes;
using System;

namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object that encodes a <see cref="RecordModes.RecordMode"/> as its type name
    /// for serialization and deserialization across the API boundary.
    /// </summary>
    public class RecordModeDTO
    {
        public string RecordMode { get; set; }

        /// <summary>
        /// Converts this DTO into the appropriate <see cref="RecordModes.RecordMode"/> subtype
        /// based on the value of <see cref="RecordMode"/>.
        /// </summary>
        /// <returns>A concrete <see cref="RecordModes.RecordMode"/> instance matching <see cref="RecordMode"/>.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="RecordMode"/> does not correspond to a known record mode.
        /// </exception>
        public RecordMode ToRecordMode()
        {
            return this.RecordMode switch
            {
                nameof(NoRecording) => this.LoadNoRecording(),
                nameof(RecordAlways) => this.LoadRecordAlways(),
                nameof(RecordOnMovements) => this.LoadRecordOnMovements(),
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
        private RecordOnMovements LoadRecordOnMovements()
        {
            RecordOnMovements result = new RecordOnMovements();
            //TODO set properties
            return result;
        }
    }
}
