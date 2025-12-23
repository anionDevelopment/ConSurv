using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;

namespace ConSurvBackend.Tests.TestUtilities
{
    public sealed class DatabaseTestFrameworkForMariaDB : DatabaseTestFrameworkTemplate
    {
        public DatabaseTestFrameworkForMariaDB(IGRYLog log) : base("ConSurv_database_mariadb", new DatabasePersistenceConfiguration() { DatabaseType = "MariaDB", DatabaseConnectionString = Utilities.GetTestMariaDBConnectionString() }, Utilities.GetTestMariaDBDatabaseFolder(), ConSurvBackend.Tests.TestUtilities.Constants.GeneralConstants.RepositoryFolder, "LocaltestserviceMariadbStart", "LocaltestserviceMariadbStop", ConSurvBackend.Tests.TestUtilities.Utilities.GetResetDatabaseScript("MariaDB"), log)
        {
        }

        public override string GetDatabaseTypeName()
        {
            return "MariaDB";
        }
    }
}
