using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Model.Base
{
    public class ProcessInformation
    {
        public string Purpose;
        public ExternalProgramExecutor ExternalProgramExecutor;

        public ProcessInformation(string purpose, ExternalProgramExecutor externalProgramExecutor)
        {
            this.Purpose = purpose;
            this.ExternalProgramExecutor = externalProgramExecutor;
        }

        public ProcessInformationDTO ToDTO()
        {
            return new ProcessInformationDTO(this.Purpose, (uint)this.ExternalProgramExecutor.ProcessId, this.ExternalProgramExecutor.Configuration.Program, Misc.Utilities.EscapeBasicAuthPasswords(this.ExternalProgramExecutor.Configuration.Argument));
        }
    }
}
