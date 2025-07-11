using ConSurvBackend.Core.Database;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    public class DatabasePostgreSQLTests : DatabasePersistenceTestsBase
    {

        public override IDatabaseManager GetDatabaseManager()
        {
            return new DatabaseManagerPostgreSQL();
        }

        public override GenericPersistence GetPersistenceObject(IDatabaseManager databaseManager, ITimeService timeService, DbContextOptionsBuilder<DatabaseContext> optionsBuilder, IGRYLog logger, ISQLProvider sqlProvider)
        {
            return new DatabasePostgreSQLPersistence(optionsBuilder.Options, logger, timeService, databaseManager, logger, sqlProvider);
        }

        public override ISQLProvider GetSQLProvider(IGRYLog log)
        {
            return new SQLProviderPostgreSQL(log);
        }

        public override DatabaseTestFrameworkTemplate GetTestFramework()
        {
            return new DatabaseTestFrameworkForPostgreSQL();
        }


        [TestMethod(nameof(AddCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void AddCameraTest()
        {
            this.AddCamera();
        }
    }
}
