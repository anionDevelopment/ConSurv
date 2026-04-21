using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
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
        private readonly IGRYLog _Log;
        private readonly IApplicationConstants<CodeUnitSpecificConstants> _Constants;
        private readonly ISet<ProcessInformation> _Processes;
        private readonly CommandlineParameter _CMDParameter;    
        public ProcessManager(IGRYLog log, IApplicationConstants<CodeUnitSpecificConstants> applicationConstants,CommandlineParameter cmdParameter)
        {
            this._Log = log;
            this._Constants = applicationConstants;
            this._Processes = new HashSet<ProcessInformation>();
            this._CMDParameter = cmdParameter;
        }
        public ExternalProgramExecutor GetBackgroundProcess(string program, string argument, string? workingFolder, Action<Process>? configureProcess, string purpose, string purposeForLogfile, bool runSynchronous)
        {
            bool verbose = false;
            if (_CMDParameter.Verbose)
            {
                verbose= true;
            }
            string workingDirectory = workingFolder ?? Directory.GetCurrentDirectory();
            DateTime now = DateTime.Now;
            string processId = Guid.NewGuid().ToString()[..8];
            string logfile = Path.Combine(this._Constants.GetLogFolder(), $"Background-process_{GUtilities.DateTimeForFilename(now)}_{purposeForLogfile}_{processId}.log");
            GRYLog eLog = GRYLog.Create(logfile);
            foreach (GRYLogTarget item in eLog.Configuration.LogTargets)
            {
                item.Enabled = true;
                if (verbose)
                {
                    item.LogLevels.Add(Microsoft.Extensions.Logging.LogLevel.Debug);
                }
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
                Verbosity = verbose ? Verbosity.Full : Verbosity.Normal
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
            string message = $"Started background process \"{workingDirectory}>{program} {argument}\" (Purpose: {purpose}; Process-id: {processId}; Technical process-id: {e.ProcessId})";
            eLog.Log(message, Microsoft.Extensions.Logging.LogLevel.Debug);
            this._Log.Log(message, Microsoft.Extensions.Logging.LogLevel.Debug);
            if (this._Constants.Environment is Development)
            {
                string processListFile = Path.Combine(this._Constants.GetConfigurationFolder(), "StartedProcesses.txt");
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(processListFile);
                GRYLibrary.Core.Misc.Utilities.AppendLineToFile(processListFile, e.ProcessId.ToString(), Misc.Utilities._Encoding);
            }
            this._Processes.Add(new ProcessInformation(purpose, processId, e));
            return e;
        }

        public ISet<ProcessInformation> GetRunningProcesses()
        {
            return this._Processes.ToHashSet();
        }

    }
}
