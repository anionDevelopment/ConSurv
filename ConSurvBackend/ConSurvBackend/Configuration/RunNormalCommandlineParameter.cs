using CommandLine;
using GRYLibrary.Core.APIServer.Verbs;

namespace ConSurvBackend.Core.Configuration
{
    [Verb(nameof(RunServer), isDefault: true, HelpText = "Runs the server.")]
    public  class RunNormalCommandlineParameter : CommandlineParameter
    {
        [Option(nameof(TestRun), Required = false, Default = false)]
        public bool TestRun { get; set; }
    }
}
