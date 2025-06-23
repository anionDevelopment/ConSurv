namespace ConSurvBackend.Core.Model.DTOs
{
    public class ProcessInformationDTO
    {
        public string Purpose { get; set; }
        public string ProcessID { get; set; }
        public uint TechnicalProcessID { get; set; }
        public string Program { get; set; }
        public string Argument{ get; set; }

        public ProcessInformationDTO(string purpose, string processID, uint technicalProcessID, string program, string argument)
        {
            this.Purpose = purpose;
            this.ProcessID = processID;
            this.TechnicalProcessID = technicalProcessID;
            this.Program = program;
            this.Argument = argument;
        }
    }
}
