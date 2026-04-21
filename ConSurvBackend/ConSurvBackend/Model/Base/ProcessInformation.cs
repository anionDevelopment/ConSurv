using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Model.Base
{
    /// <summary>
    /// Holds runtime metadata about an external process managed by ConSurv,
    /// such as an FFmpeg or MediaMTX process associated with a camera stream.
    /// </summary>
    public class ProcessInformation
    {
        public string Purpose;
        public string ProcessId;
        public ExternalProgramExecutor ExternalProgramExecutor;

        /// <summary>
        /// Initializes a new <see cref="ProcessInformation"/> instance.
        /// </summary>
        /// <param name="purpose">A human-readable description of why this process was started.</param>
        /// <param name="processId">The application-level identifier assigned to this process.</param>
        /// <param name="externalProgramExecutor">The executor that owns and controls the OS process.</param>
        public ProcessInformation(string purpose,string processId, ExternalProgramExecutor externalProgramExecutor)
        {
            this.Purpose = purpose;
            this.ExternalProgramExecutor = externalProgramExecutor;
            this.ProcessId=processId;
        }

        /// <summary>
        /// Creates a <see cref="ProcessInformationDTO"/> snapshot of this instance suitable for
        /// transfer to clients. Basic-auth passwords in the command-line argument are escaped
        /// before being included in the DTO.
        /// </summary>
        /// <returns>A DTO representing the current state of this process.</returns>
        public ProcessInformationDTO ToDTO()
        {
            return new ProcessInformationDTO(this.Purpose, this.ProcessId,(uint)this.ExternalProgramExecutor.ProcessId, this.ExternalProgramExecutor.Configuration.Program, Misc.Utilities.EscapeBasicAuthPasswords(this.ExternalProgramExecutor.Configuration.Argument));
        }
    }
}
