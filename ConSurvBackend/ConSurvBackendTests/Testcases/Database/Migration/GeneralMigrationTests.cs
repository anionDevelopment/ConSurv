using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc.Migration;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ConSurvBackend.Tests.TestUtilities;
using ConSurvBackend.Core.Database;
using GRYLibrary.Core.APIServer.Services.OtherServices;

namespace ConSurvBackend.Tests.Testcases.Database.Migration
{
    [TestClass]
    public class GeneralMigrationTests
    {
        [TestMethod(nameof(AllMigrationsAreWorking))]
        [TestProperty(nameof(TestKind), nameof(TestKind.IntegrationTest))]
        public void AllMigrationsAreWorking()
        {
            //arrange
            using DatabaseTestFramework databaseTestFramework = new DatabaseTestFramework();
            IDatabaseManager databaseManager = new DatabaseManager();
            IList<MigrationInstance> migrations = databaseManager.GetAllMigrations();
            Assert.IsFalse(databaseManager.GetGenericDatabaseInteractor().GetAllTableNames(databaseTestFramework.MySqlConnection).Any());
            GRYMigrator migrator = new GRYMigrator(GeneralLogger.CreateUsingConsole(), new TimeService(), databaseTestFramework.MySqlConnection, migrations, databaseManager.GetGenericDatabaseInteractor());

            //act
            migrator.InitializeDatabaseAndMigrateIfRequired();

            //assert
            Assert.IsTrue(databaseManager.GetGenericDatabaseInteractor().GetAllTableNames(databaseTestFramework.MySqlConnection).Any());
            Assert.AreEqual(1, migrator.GetExecutedMigrations().Count);
        }
    }
}
