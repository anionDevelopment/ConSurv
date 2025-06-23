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
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Services
{
    public class ProcessManager : IProcessManager
    {
        private IGRYLog _Log;
        private IApplicationConstants _ApplicationConstants;
        private ISet<ProcessInformation> _Processes;
        public ProcessManager(IGRYLog log, IApplicationConstants applicationConstants)
        {
            this._Log = log;
            this._ApplicationConstants = applicationConstants;
            _Processes = new HashSet<ProcessInformation>();
        }
        public ExternalProgramExecutor GetBackgroundProcess(string program, string argument, string? workingFolder, Action<Process>? configureProcess, string purpose, string purposeForLogfile, bool runSynchronous)
        {
            bool verbose = true;//TODO should only be true in verbose mode
            string workingDirectory = workingFolder ?? Directory.GetCurrentDirectory();
            DateTime now = DateTime.Now;
            string processId = Guid.NewGuid().ToString().Substring(0, 8);
            string logfile = Path.Combine(_ApplicationConstants.GetLogFolder(), $"Background-process_{GUtilities.DateTimeForFilename(now)}_{purposeForLogfile}_{processId}.log");
            GRYLog eLog = GRYLog.Create(logfile);
            foreach (GRYLogTarget item in eLog.Configuration.LogTargets)
            {
                item.Enabled = true;
                item.LogLevels.Add(Microsoft.Extensions.Logging.LogLevel.Debug);
                item.LogLevels.Add(Microsoft.Extensions.Logging.LogLevel.Information);
                item.LogLevels.Add(Microsoft.Extensions.Logging.LogLevel.Warning);
                item.LogLevels.Add(Microsoft.Extensions.Logging.LogLevel.Error);
                if (item is GRYLibrary.Core.Logging.GRYLogger.ConcreteLogTargets.Console)
                {
                    item.Enabled = false;
                }
            }
            ExternalProgramExecutor e = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
            {
                Program = program,
                Argument = argument,
                WorkingDirectory = workingDirectory,
                LogFile = logfile,
                Verbosity = verbose ? Verbosity.Verbose : Verbosity.Normal
            })
            {
                LogObject = eLog
            };

            if (runSynchronous)
            {
                e.Configuration.WaitingState = new RunSynchronously();
            }
            else
            {
                e.Configuration.WaitingState = new RunAsynchronously();
            }
            e.Run();
            string message = $"Started background process \"{workingDirectory}>{program} {argument}\" (Purpose: {purpose}; Process-id: {processId}; Technical process-id: {e._Process.Id})";
            eLog.Log(message, Microsoft.Extensions.Logging.LogLevel.Debug);
            _Log.Log(message, Microsoft.Extensions.Logging.LogLevel.Debug);
            if (_ApplicationConstants.Environment is Development)
            {
                string processListFile = Path.Combine(_ApplicationConstants.GetConfigurationFolder(), "StartedProcesses.txt");
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(processListFile);
                GRYLibrary.Core.Misc.Utilities.AppendLineToFile(processListFile, e.ProcessId.ToString(), Misc.Utilities._Encoding);
            }
            _Processes.Add(new ProcessInformation(purpose, processId, e));
            return e;
        }

        public ISet<ProcessInformation> GetRunningProcesses()
        {
            return _Processes.ToHashSet();
        }

    }
}
