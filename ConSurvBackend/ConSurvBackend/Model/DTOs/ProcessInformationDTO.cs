namespace ConSurvBackend.Core.Model.DTOs
{
    public class ProcessInformationDTO
    {
        public string Purpose { get; set; }
        public uint ProcessID{ get; set; }
        public string Program { get; set; }
        public string Argument{ get; set; }

        public ProcessInformationDTO(string purpose, uint processID, string program, string argument)
        {
            this.Purpose = purpose;
            this.ProcessID = processID;
            this.Program = program;
            this.Argument = argument;
        }
    }
}
