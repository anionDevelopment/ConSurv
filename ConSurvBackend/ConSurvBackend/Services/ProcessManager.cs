using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConSurvBackend.Core.Services
{
    public class ProcessManager: IProcessManager
    {
        private IGRYLog _Log;
        private IApplicationConstants _ApplicationConstants;
        private ISet<ProcessInformation> _Processes;
        public ProcessManager(IGRYLog log, IApplicationConstants applicationConstants)
        {
            this._Log = log;
            this._ApplicationConstants = applicationConstants;
            _Processes=new HashSet<ProcessInformation>();
        }
        public ExternalProgramExecutor GetBackgroundProcess(string program, string argument, string? workingFolder,  Action<Process>? configureProcess,  string purpose, bool runSynchronous)
        {
            bool verbose = true;//can be changed to trkue temporary for debugging purposes
            string workingDirectory = workingFolder ?? Directory.GetCurrentDirectory();
            ExternalProgramExecutor e = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
            {
                Program = program,
                Argument = argument,
                WorkingDirectory = workingDirectory,

            });
            _Log.Log($"Started background process \"{workingDirectory}>{program} {argument}\" (Purpose: {purpose})", Microsoft.Extensions.Logging.LogLevel.Debug);
            e.Configuration.Verbosity = verbose ? Verbosity.Verbose : Verbosity.Quiet;
            if (runSynchronous)
            {
                e.Configuration.WaitingState = new RunSynchronously();
            }
            else
            {
                e.Configuration.WaitingState = new RunAsynchronously();
            }
            e.Run();
            if (_ApplicationConstants.Environment is Development)
            {
                string processListFile = Path.Combine(_ApplicationConstants.GetConfigurationFolder(), "StartedProcesses.txt");
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(processListFile);
                GRYLibrary.Core.Misc.Utilities.AppendLineToFile(processListFile, e.ProcessId.ToString(), Misc.Utilities._Encoding);
            }
            _Processes.Add(new ProcessInformation(purpose,e));
            return e;
        }

        public ISet<ProcessInformation> GetRunningProcesses()
        {
            return _Processes.ToHashSet();
        }

    }
}
