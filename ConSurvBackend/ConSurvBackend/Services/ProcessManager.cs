using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConSurvBackend.Core.Services
{
    public sealed class ProcessManager : IProcessManager, IDisposable
    {
        private readonly ISet<Process> _Processes = new HashSet<Process>();
        private readonly IGeneralLogger _Logger;
        public ProcessManager(IGeneralLogger logger)
        {
            this._Logger = logger;
        }
        public void Dispose()
        {
            foreach (var process in this._Processes)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        this._Logger.Log($"Try kill process with id {process.Id}...", Microsoft.Extensions.Logging.LogLevel.Information);
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    this._Logger.LogException(ex, $"Error while kill process with id {process.Id}.");
                }
            }
        }

        public void RegisterProcess(Process process)
        {
            this._Processes.Add(process);
        }
    }
}
