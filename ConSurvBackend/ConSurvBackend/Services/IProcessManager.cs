using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConSurvBackend.Core.Services
{
    public interface IProcessManager
    {
        public ExternalProgramExecutor GetBackgroundProcess(string program, string argument, string? workingFolder, Action<Process>? configureProcess, string purpose, string purposeForLogfile, bool runSynchronous);
        public ISet<ProcessInformation> GetRunningProcesses();
    }
}
