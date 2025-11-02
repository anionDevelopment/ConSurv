using CommandLine;
using GRYLibrary.Core.APIServer.Verbs;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Configuration { 
    public  class CommandlineParameter : RunServer
    {

        [Option(nameof(InitialAdminPassword), Required = false)]
        public string? InitialAdminPassword { get; set; }

        [Option(nameof(InitialDatabaseType), Required = false)]
        public string? InitialDatabaseType { get; set; }

        [Option(nameof(InitialDatabaseConnectionString), Required = false)]
        public string? InitialDatabaseConnectionString { get; set; }

        [Option(nameof(InitialCameraAddresses), Required = false, Separator = ';')]
        public IEnumerable<string> InitialCameraAddresses { get; set; }
    }
}
