using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConSurvBackend.Core.Services
{
    public interface IProcessManager
    {
        /// <summary>
        /// Starts an external program as a managed background process and registers it internally.
        /// </summary>
        /// <param name="program">The name or path of the executable to run.</param>
        /// <param name="argument">The command-line arguments to pass to the executable.</param>
        /// <param name="workingFolder">The working directory for the process, or <c>null</c> to use the current directory.</param>
        /// <param name="configureProcess">An optional callback to further configure the <see cref="Process"/> before it is started.</param>
        /// <param name="purpose">A human-readable description of why the process is being started, used for logging.</param>
        /// <param name="purposeForLogfile">A short identifier appended to the logfile name for this process.</param>
        /// <param name="runSynchronous">If <c>true</c>, the call blocks until the process exits; otherwise it runs asynchronously.</param>
        /// <returns>The <see cref="ExternalProgramExecutor"/> representing the started process.</returns>
        public ExternalProgramExecutor GetBackgroundProcess(string program, string argument, string? workingFolder, Action<Process>? configureProcess, string purpose, string purposeForLogfile, bool runSynchronous);

        /// <summary>
        /// Returns a snapshot of all processes started by this manager.
        /// </summary>
        /// <returns>A set of <see cref="ProcessInformation"/> objects describing the managed processes.</returns>
        public ISet<ProcessInformation> GetRunningProcesses();
    }
}
