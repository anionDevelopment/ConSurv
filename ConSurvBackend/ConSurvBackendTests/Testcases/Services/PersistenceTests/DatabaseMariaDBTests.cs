using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Database;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    public class DatabaseMariaDBTests : DatabasePersistenceTestsBase
    {
        public override IDatabaseManager GetDatabaseManager()
        {
            return new DatabaseManagerMariaDB();
        }


        public override GenericDatabasePersistence GetPersistenceObject(IDatabaseManager databaseManager, ITimeService timeService, DbContextOptionsBuilder<DatabaseContext> optionsBuilder, IGRYLog logger, ISQLProvider sqlProvider, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> persistedAPIServerConfiguration)
        {
            return new DatabaseMariaDBPersistence(optionsBuilder.Options,  timeService, databaseManager, logger, sqlProvider, persistedAPIServerConfiguration);
        }

        public override ISQLProvider GetSQLProvider(IGRYLog log)
        {
            return new SQLProviderMariaDB(log);
        }

        public override DatabaseTestFrameworkTemplate GetTestFramework()
        {
            return new DatabaseTestFrameworkForMariaDB();
        }


        [TestMethod(nameof(AddCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void AddCameraTest()
        {
            this.AddCamera();
        }
    }
}
