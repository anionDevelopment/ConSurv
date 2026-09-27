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

        /// <summary>The color of the background. It has to be the one which UpdateRecordedStreamBaselines.py uses, because a picture of a recording is compared with a picture which that script generated.</summary>
        private const string _BackgroundColor = "dodgerblue";

        /// <summary>
        /// The size of the picture. It has to be the one which UpdateRecordedStreamBaselines.py uses.
        /// </summary>
        /// <remarks>
        /// It is deliberately wider than the default of the generator: the application draws the name and the id of
        /// the camera and the current time into the stream, and that text is 1417 pixels wide (measured), so most of
        /// it would be cut off at the left in a picture which has the default-width of 640 pixels.
        /// </remarks>
        private const int _PictureWidth = 1920;
        private const int _PictureHeight = 1080;

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
                $"print(ScriptCollectionCore().start_rtsp_test_stream(r'{address}', r'{name}', width={_PictureWidth}, height={_PictureHeight}, font_file=r'{_FontFile}', background_color='{_BackgroundColor}'))";
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
                // Waiting for the exit is not possible here: the ffmpeg is not a child-process of the test-process
                // (ScriptCollection starts it and the python-process which started it ends immediately), so on linux it
                // stays as a zombie until the process with the id 1 reaps it, which the build-script does not do.
                // Process.WaitForExit then does not return anymore, not even after the timeout which is passed to it.
                // Killing the process ends the stream, which is what has to happen here.
                process.Kill(true);
            }
            catch (ArgumentException)
            {
                // The process does not exist anymore, which is what should happen here anyway.
            }
        }
    }
}
