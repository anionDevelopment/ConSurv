using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities.Constants;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Tests.TestUtilities
{
    public static class Utilities
    {
        internal static readonly object LockForTests = new object();
        public static string GetResetDatabaseScript(string databaseName)
        {
            string file = Path.Combine(ConSurvBackend.Tests.TestUtilities.Constants.GeneralConstants.CodeUnitFolder, "ConSurvBackend", "Resources", "Database", databaseName, "Statements", "ResetDatabase.sql");
            string result = File.ReadAllText(file, new UTF8Encoding(false));
            return result;
        }
        public static (TransientPersistence, ISet<IDisposable>) GetTransientPersistence()
        {
            ITimeService timeService = new TimeService();
            TransientAuthenticationServicePersistence<User> transientAuthenticationServicePersistence = new TransientAuthenticationServicePersistence<User>(timeService);
            TransientPersistence persistence = new TransientPersistence(transientAuthenticationServicePersistence);
            return (persistence, new HashSet<IDisposable>());
        }

        public static string GetTestMariaDBDatabaseFolder()
        {
            return GUtilities.ResolveToFullPath(@$"{GeneralConstants.RepositoryFolder}\Other\Resources\LocalTestServices\MariaDBDatabase");
        }

        /// <summary>
        /// Returns the host under which the test-database-container is reachable.
        /// Inside the build-container the test-process is attached to the docker-network of the test-services and reaches the container directly by its
        /// container-name (no ports are published to the host). Locally the container's ports are published, so it is reached via "localhost".
        /// </summary>
        private static string GetTestDatabaseHost()
        {
            bool runningInBuildContainer = System.Environment.GetEnvironmentVariable("ISRUNNINGINBUILDCONTAINER") == "true";
            return runningInBuildContainer ? "consurv_database" : "localhost";
        }

        public static string GetTestMariaDBConnectionString()
        {
            return @$"Host={GetTestDatabaseHost()};Port=3306;User ID=user;Password=pa55w0rd;Database=ConSurvDatabase;";
        }
        public static string GetTestPostgreSQLDatabaseFolder()
        {
            return GUtilities.ResolveToFullPath(@$"{GeneralConstants.RepositoryFolder}\Other\Resources\LocalTestServices\PostgreSQLDatabase");
        }

        public static string GetTestPostgreSQLConnectionString()
        {
            return @$"Host={GetTestDatabaseHost()};Port=5432;Username=user;Password=pa55w0rd;Database=ConSurvDatabase;IncludeErrorDetail=true;";
        }

        public static string GetTestDatabaseCreationScriptArtifactFolder(string databaseName)
        {
            return GUtilities.ResolveToFullPath(@$"{GeneralConstants.CodeUnitFolder}\Other\Artifacts\${databaseName}DatabaseCreationScript");
        }

        public static DatabaseTestFrameworkForMariaDB GetDatabaseTestFrameworkForMariaDB()
        {
            return new DatabaseTestFrameworkForMariaDB(GeneralLogger.CreateUsingConsole());
        }
        public static DatabaseTestFrameworkForPostgreSQL GetDatabaseTestFrameworkForPostgreSQL()
        {
            return new DatabaseTestFrameworkForPostgreSQL(GeneralLogger.CreateUsingConsole());
        }

        public static DatabaseInteractorMariaDB GetMariaDBTestDatabase()
        {
            return new DatabaseInteractorMariaDB(DBUtilities.ToGenericDatabaseInteractor(GetDatabaseConfigurationMariaDB(), GeneralLogger.CreateUsingConsole()));
        }

        public static DatabaseInteractorPostgreSQL GetPostgreSQLTestDatabase()
        {
            return new DatabaseInteractorPostgreSQL(DBUtilities.ToGenericDatabaseInteractor(GetDatabaseConfigurationPostgreSQL(), GeneralLogger.CreateUsingConsole()));
        }
        public static IDatabasePersistenceConfiguration GetDatabaseConfigurationMariaDB()
        {
            return new DatabasePersistenceConfiguration()
            {
                DatabaseType = "MariaDB",
                DatabaseConnectionString = ConSurvBackend.Tests.TestUtilities.Utilities.GetTestMariaDBConnectionString()
            };
        }

        public static IDatabasePersistenceConfiguration GetDatabaseConfigurationPostgreSQL()
        {
            return new DatabasePersistenceConfiguration()
            {
                DatabaseType = "PostgreSQL",
                DatabaseConnectionString = ConSurvBackend.Tests.TestUtilities.Utilities.GetTestPostgreSQLConnectionString()
            };
        }
    }
}
