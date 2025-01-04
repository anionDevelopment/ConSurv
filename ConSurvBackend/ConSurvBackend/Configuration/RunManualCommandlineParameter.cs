using CommandLine;
using GRYLibrary.Core.APIServer.Verbs;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Configuration
{
    [Verb(nameof(RunServer)+"Manual", isDefault: false, HelpText = "Runs the server with a custom readonly initial configuration.")]
    public  class RunManualCommandlineParameter :CommandlineParameter
    {
        [Option(nameof(TestRun), Required = false, Default = false)]
        public bool TestRun { get; set; }
        [Option(nameof(AdminPassword), Required = true)]
        public string AdminPassword { get; set; }
        [Option(nameof(CameraAddresses), Required = true)]
        public IEnumerable<string> CameraAddresses { get; set; }
    }
}
