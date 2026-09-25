using GRYLibrary.Core.APIServer.Settings;
using System;

namespace ConSurvBackend.Tests.TestUtilities
{
    public class IntegrationTestConfiguration
    {
        public bool RunInOwnThread { get; set; }

        /// <summary>Whether the started application runs its background-services. They are switched off in a test-run unless a testcase asks for them.</summary>
        public bool RunBackgroundProcesses { get; set; }
        public Action<FunctionalInformation<ConSurvBackend.Core.Constants.CodeUnitSpecificConstants, ConSurvBackend.Core.Configuration.CodeUnitSpecificConfiguration, ConSurvBackend.Core.Configuration.CommandlineParameter>>? SetupMocks { get; set; }
        public IntegrationTestConfiguration(Action<FunctionalInformation<ConSurvBackend.Core.Constants.CodeUnitSpecificConstants, ConSurvBackend.Core.Configuration.CodeUnitSpecificConfiguration, ConSurvBackend.Core.Configuration.CommandlineParameter>>? setupMocks = null, bool runInOwnThread = false)
        {
            this.SetupMocks = setupMocks;
            this.RunInOwnThread = runInOwnThread;
        }
    }
}
