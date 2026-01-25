using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Tests.TestUtilities
{
    public sealed class DatabaseTestFrameworkForMariaDB : DatabaseTestFrameworkTemplate
    {
        public DatabaseTestFrameworkForMariaDB(IGRYLog log) : base(new DatabasePersistenceConfiguration() { DatabaseType = "MariaDB", DatabaseConnectionString = Utilities.GetTestMariaDBConnectionString() }, Utilities.GetTestMariaDBDatabaseFolder(), ConSurvBackend.Tests.TestUtilities.Constants.GeneralConstants.RepositoryFolder, "LocaltestserviceMariadbStart", "LocaltestserviceMariadbStop", ConSurvBackend.Tests.TestUtilities.Utilities.GetResetDatabaseScript("MariaDB"), log, TimeSpan.FromSeconds(200))
        {
        }

        public override string GetDatabaseTypeName()
        {
            return "MariaDB";
        }
    }
}
