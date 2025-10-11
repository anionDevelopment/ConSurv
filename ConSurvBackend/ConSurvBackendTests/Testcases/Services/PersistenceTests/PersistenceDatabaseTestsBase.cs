using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    public abstract class PersistenceDatabaseTestsBase : PersistenceTestsBase
    {
        protected abstract DatabaseTestFrameworkTemplate GetDatabaseTestFramework();
        public override IPersistence GetPersistence()
        {
            DatabaseTestFrameworkTemplate databaseTestFramework = this.GetDatabaseTestFramework();
            ITimeService timeService = new TimeService();
            IGRYLog logger = GeneralLogger.CreateUsingConsole();
            IPersistence result = new DatabasePersistence(databaseTestFramework.GenericDatabaseInteractor().Accept(new GetConSurvDatabaseInteractorVisitor()), timeService, logger);


            databaseTestFramework.ResetDatabase();
            IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
            IConSurvDatabaseInteractor ConSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

            List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
            Assert.IsEmpty(tables1);

            IList<MigrationInstance> migrations = ConSurvDatabaseInteractor.GetAllMigrations();
            GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.ToList(), databaseInteractor);

            migrator.InitializeDatabaseAndMigrateIfRequired();

            return result;
        }
    }
}
