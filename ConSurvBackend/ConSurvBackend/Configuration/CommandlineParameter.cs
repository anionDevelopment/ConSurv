using CommandLine;
using GRYLibrary.Core.APIServer.Verbs;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Configuration
{
    [Verb(nameof(RunServer), isDefault: true, HelpText = "Runs the server.")]
    public  class CommandlineParameter : IAPIServerCommandlineParameter
    {
        [Option(nameof(TestRun), Required = false, Default = false)]
        public bool TestRun { get; set; }

        [Option(nameof(InitialAdminPassword), Required = false)]
        public string? InitialAdminPassword { get; set; }

        [Option(nameof(InitialCameraAddresses), Required = false)]
        public IEnumerable<string> InitialCameraAddresses { get; set; }
    }
}
