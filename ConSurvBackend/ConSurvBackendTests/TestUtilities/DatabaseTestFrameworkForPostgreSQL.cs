using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.TestUtilities
{
    public sealed class DatabaseTestFrameworkForPostgreSQL : DatabaseTestFrameworkTemplate
    {
        public DatabaseTestFrameworkForPostgreSQL(IGRYLog log) : base(new DatabasePersistenceConfiguration() { DatabaseType = "PostgreSQL", DatabaseConnectionString = Utilities.GetTestPostgreSQLConnectionString() }, Utilities.GetTestPostgreSQLDatabaseFolder(), ConSurvBackend.Tests.TestUtilities.Constants.GeneralConstants.RepositoryFolder, "LocaltestservicePostgresqlStart", "LocaltestservicePostgresqlStop", ConSurvBackend.Tests.TestUtilities.Utilities.GetResetDatabaseScript("PostgreSQL"), log, TimeSpan.FromSeconds(200), new HashSet<string>() { "consurv_database" })
        {
        }

        public override string GetDatabaseTypeName()
        {
            return "PostgreSQL";
        }
    }
}
