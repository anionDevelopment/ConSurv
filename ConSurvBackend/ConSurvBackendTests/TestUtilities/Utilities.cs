using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities.Constants;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Services.Trans;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Tests.TestUtilities
{
    public static class Utilities
    {
        public static TransientPersistence GetTransientPersistence()
        {
            ITimeService timeService = new TimeService();
            TransientAuthenticationServicePersistence<User> transientAuthenticationServicePersistence = new TransientAuthenticationServicePersistence<User>(timeService);
            TransientPersistence persistence = new TransientPersistence(transientAuthenticationServicePersistence);
            return persistence;
        }
        public static string GetTestMariaDBDatabaseFolder()
        {
            return GUtilities.ResolveToFullPath(@$"{GeneralConstants.RepositoryFolder}\Other\Resources\LocalTestServices\MariaDBDatabase");
        }
        public static string GetTestPostgreSQLDatabaseFolder()
        {
            return GUtilities.ResolveToFullPath(@$"{GeneralConstants.RepositoryFolder}\Other\Resources\LocalTestServices\PostgreSQLDatabase");
        }
        public static string GetTestDatabaseCreationScriptArtifactFolder(string databaseName)
        {
            return GUtilities.ResolveToFullPath(@$"{GeneralConstants.CodeUnitFolder}\Other\Artifacts\{databaseName}DatabaseCreationScript");
        }
    }
}
