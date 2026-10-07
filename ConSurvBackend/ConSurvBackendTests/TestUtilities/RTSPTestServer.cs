using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConSurvBackend.Tests.TestUtilities
{
    /// <summary>
    /// A rtsp-server for testcases. The test-streams publish to it and the cameras of the application read from
    /// it, so a testcase can work with a camera without a camera existing.
    ///
    /// It is a mediamtx of its own and not the mediamtx which the application starts per camera: that one is fed
    /// by the application itself, so publishing to it would replace exactly the part which is supposed to be tested.
    /// </summary>
    public sealed class RTSPTestServer : IDisposable
    {
        private readonly ExternalProgramExecutor _Process;
        /// <summary>
        /// The folder in which the server runs. It lies in the temp-folder of the operating-system and not in this
        /// repository, because mediamtx writes into its working-directory: it creates the self-signed certificate
        /// "auto.crt" and its key "auto.key" there when it starts. In the folder of the resources those files would
        /// stay behind, the build would copy them into every build-result, and the secret-scan of the build-pipeline
        /// would report the key.
        /// </summary>
        private readonly string _WorkingFolder;

        /// <summary>The port on which the server accepts rtsp-connections.</summary>
        public ushort Port { get; }

        /// <param name="pathNames">The names of the paths which the server offers. A test-stream publishes to one of them.</param>
        public RTSPTestServer(params string[] pathNames)
        {
            this.Port = GetFreePort();
            string executable = GetExecutable();
            StringBuilder configuration = new StringBuilder();
            // Every server of mediamtx which is not switched off here listens on a port of its own which this
            // configuration does not set. This server runs at the same time as the media-hubs of the application,
            // so an enabled one of them would take that port away from them and they could not start.
            configuration.Append($"rtspAddress: \"127.0.0.1:{this.Port}\"\nrtmp: no\nhls: no\nwebrtc: no\nsrt: no\nmoq: no\nrtspTransports: [tcp]\npaths:\n");
            foreach (string pathName in pathNames)
            {
                // "overridePublisher" allows a stream to be started again while the server is running, which is
                // what a testcase does when it wants a stream to begin at zero again.
                configuration.Append($"  {pathName}:\n    overridePublisher: yes\n");
            }
            this._WorkingFolder = Path.Combine(Path.GetTempPath(), $"ConSurvRTSPTestServer_{Guid.NewGuid():N}");
            Directory.CreateDirectory(this._WorkingFolder);
            string configurationFile = Path.Combine(this._WorkingFolder, "MediaMTXTestConfiguration.yml");
            File.WriteAllText(configurationFile, configuration.ToString(), new UTF8Encoding(false));
            this._Process = new ExternalProgramExecutor(executable, Path.GetFileName(configurationFile), this._WorkingFolder);
            // The server has to keep running while the testcase uses it, so it is not waited for.
            this._Process.Configuration.WaitingState = new RunAsynchronously();
            this._Process.Run();
            this.WaitUntilTheServerAcceptsConnections();
        }

        /// <summary>Returns the rtsp-address of the given path.</summary>
        public string GetAddressOfPath(string pathName)
        {
            return $"rtsp://127.0.0.1:{this.Port}/{pathName}";
        }

        private void WaitUntilTheServerAcceptsConnections()
        {
            // The server needs a moment until it listens. Without waiting, the first stream would be started
            // against a port which is not open yet and would fail.
            DateTime timeout = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            while (DateTime.UtcNow < timeout)
            {
                try
                {
                    using TcpClient client = new TcpClient();
                    client.Connect(IPAddress.Loopback, this.Port);
                    return;
                }
                catch (SocketException)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(200));
                }
            }
            throw new TimeoutException($"The rtsp-test-server did not accept connections on port {this.Port}.");
        }

        /// <summary>Returns the mediamtx which belongs to the current operating-system. It is the one which is delivered with this codeunit, so no testcase depends on a mediamtx being installed on the machine.</summary>
        private static string GetExecutable()
        {
            bool runningOnWindows = GRYLibrary.Core.OperatingSystem.OperatingSystem.GetCurrentOperatingSystem() is GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems.Windows;
            string nameOfTheResourceFolder = runningOnWindows ? "MediaMTX_Windows-x64" : "MediaMTX_Linux-x64";
            string folder = Path.Combine(Constants.GeneralConstants.CodeUnitFolder, "Other", "Resources", nameOfTheResourceFolder, "MediaMTX");
            string executable = Path.Combine(folder, runningOnWindows ? "mediamtx.exe" : "mediamtx");
            if (!File.Exists(executable))
            {
                throw new FileNotFoundException($"The mediamtx of this codeunit was not found. Run the common-tasks of the codeunit to download it.", executable);
            }
            return executable;
        }

        private static ushort GetFreePort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                return (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        public void Dispose()
        {
            try
            {
                this._Process.Dispose();
            }
            finally
            {
                if (Directory.Exists(this._WorkingFolder))
                {
                    Directory.Delete(this._WorkingFolder, true);
                }
            }
        }
    }
}
