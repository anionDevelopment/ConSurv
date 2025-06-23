using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Model.Base
{
    public class ProcessInformation
    {
        public string Purpose;
        public string ProcessId;
        public ExternalProgramExecutor ExternalProgramExecutor;

        public ProcessInformation(string purpose,string processId, ExternalProgramExecutor externalProgramExecutor)
        {
            this.Purpose = purpose;
            this.ExternalProgramExecutor = externalProgramExecutor;
            this.ProcessId=processId;
        }

        public ProcessInformationDTO ToDTO()
        {
            return new ProcessInformationDTO(this.Purpose, ProcessId,(uint)this.ExternalProgramExecutor.ProcessId, this.ExternalProgramExecutor.Configuration.Program, Misc.Utilities.EscapeBasicAuthPasswords(this.ExternalProgramExecutor.Configuration.Argument));
        }
    }
}
