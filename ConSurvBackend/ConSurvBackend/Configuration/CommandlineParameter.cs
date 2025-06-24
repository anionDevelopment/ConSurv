using CommandLine;
using GRYLibrary.Core.APIServer.Verbs;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Configuration { 
    public  class CommandlineParameter : RunServer
    {

        [Option(nameof(InitialAdminPassword), Required = false)]
        public string? InitialAdminPassword { get; set; }

        [Option(nameof(InitialCameraAddresses), Required = false, Separator = ';')]
        public IEnumerable<string> InitialCameraAddresses { get; set; }
    }
}
