using GRYLibrary.Core.ExecutePrograms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConSurvBackend.Tests.TestUtilities
{
    /// <summary>
    /// A video-stream for testcases: it shows a number which is counted up once per second and, below it, the name
    /// of the stream. The picture therefore says which stream it belongs to and how long that stream is running,
    /// which is what makes a frame of a recording comparable with a baseline-picture.
    ///
    /// The stream is produced by "start_rtsp_test_stream" of ScriptCollection and not by an own ffmpeg-call, so
    /// that there is one definition of how such a stream looks.
    /// </summary>
    public sealed class RTSPTestStream : IDisposable
    {
        /// <summary>The font which is used for the picture. It is the font of this repository and not one of the operating-system, so the picture is the same on every machine and the baseline-pictures stay comparable.</summary>
        private static readonly string _FontFile = Path.Combine(Constants.GeneralConstants.RepositoryFolder, "Other", "Resources", "Fonts", "Noto", "NotoSans_Condensed-Regular.ttf");

        private readonly int _ProcessId;

        /// <summary>The name which is shown in the picture and which is the path of the stream on the server.</summary>
        public string Name { get; }

        /// <summary>The address under which the stream can be read.</summary>
        public string Address { get; }

        public RTSPTestStream(RTSPTestServer server, string name)
        {
            this.Name = name;
            this.Address = server.GetAddressOfPath(name);
            this._ProcessId = StartStream(this.Address, name);
        }

        /// <summary>Starts the stream by calling the function of ScriptCollection and returns the process-id of the ffmpeg which produces it.</summary>
        private static int StartStream(string address, string name)
        {
            if (!File.Exists(_FontFile))
            {
                throw new FileNotFoundException("The font for the test-stream was not found.", _FontFile);
            }
            string pythonStatement =
                "from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore;" +
                $"print(ScriptCollectionCore().start_rtsp_test_stream(r'{address}', r'{name}', font_file=r'{_FontFile}'))";
            using ExternalProgramExecutor executor = new ExternalProgramExecutor("python", $"-c \"{pythonStatement}\"");
            executor.Run();
            if (executor.ExitCode != 0)
            {
                throw new InvalidOperationException($"The test-stream \"{name}\" could not be started. StdErr: {string.Join(Environment.NewLine, executor.AllStdErrLines)}");
            }
            // The function prints the process-id. Other lines can be log-output, so the last line which is a
            // number is taken instead of simply the last line.
            foreach (string line in executor.AllStdOutLines.Reverse())
            {
                if (int.TryParse(line.Trim(), out int processId))
                {
                    return processId;
                }
            }
            throw new InvalidOperationException($"The test-stream \"{name}\" did not report a process-id. StdOut: {string.Join(Environment.NewLine, executor.AllStdOutLines)}");
        }

        public void Dispose()
        {
            try
            {
                Process process = Process.GetProcessById(this._ProcessId);
                process.Kill(true);
                process.WaitForExit(TimeSpan.FromSeconds(30));
            }
            catch (ArgumentException)
            {
                // The process does not exist anymore, which is what should happen here anyway.
            }
        }
    }
}
