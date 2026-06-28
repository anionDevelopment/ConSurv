namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Read-only data transfer object that exposes runtime information about a managed
    /// external process (e.g. FFmpeg or MediaMTX) to clients.
    /// </summary>
    public class ProcessInformationDTO
    {
        public string Purpose { get; set; }
        public string ProcessID { get; set; }
        public uint TechnicalProcessID { get; set; }
        public string Program { get; set; }
        public string Argument{ get; set; }

        /// <summary>
        /// Initializes a new <see cref="ProcessInformationDTO"/>.
        /// </summary>
        /// <param name="purpose">Human-readable description of the process purpose.</param>
        /// <param name="processID">Application-level process identifier.</param>
        /// <param name="technicalProcessID">OS-level process ID (PID).</param>
        /// <param name="program">Executable path or name that was launched.</param>
        /// <param name="argument">Command-line arguments passed to the program (passwords escaped).</param>
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
