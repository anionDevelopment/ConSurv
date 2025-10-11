using GRYLibrary.Core.APIServer.Settings;
using System;

namespace ConSurvBackend.Tests.TestUtilities
{
    public class IntegrationTestConfiguration
    {
        public bool RunInOwnThread { get; set; }
        public Action<FunctionalInformation<ConSurvBackend.Core.Constants.CodeUnitSpecificConstants, ConSurvBackend.Core.Configuration.CodeUnitSpecificConfiguration, ConSurvBackend.Core.Configuration.CommandlineParameter>>? SetupMocks { get; set; }
        public IntegrationTestConfiguration(Action<FunctionalInformation<ConSurvBackend.Core.Constants.CodeUnitSpecificConstants, ConSurvBackend.Core.Configuration.CodeUnitSpecificConfiguration, ConSurvBackend.Core.Configuration.CommandlineParameter>>? setupMocks = null, bool runInOwnThread = false)
        {
            this.SetupMocks = setupMocks;
            this.RunInOwnThread = runInOwnThread;
        }
    }
}
