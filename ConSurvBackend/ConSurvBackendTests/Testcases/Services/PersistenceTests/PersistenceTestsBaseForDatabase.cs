using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    public abstract class PersistenceTestsBaseForDatabase : PersistenceTestsBase
    {
        protected abstract DatabaseTestFrameworkTemplate GetDatabaseTestFramework();
        internal override PersistenceDisposable GetPersistence()
        {
            DatabaseTestFrameworkTemplate databaseTestFramework = this.GetDatabaseTestFramework();
            ITimeService timeService = new TimeService();
            IServerLog logger = ServerLog.GetTransientLog();
            Mock<IDatabasePersistenceConfiguration> databasePersistenceConfigurationMock = new Mock<IDatabasePersistenceConfiguration>(MockBehavior.Strict) ;
            databasePersistenceConfigurationMock.SetupGet(m => m.DatabaseConnectionString).Returns(databaseTestFramework.ConnectionString);
            IPersistence result = new DatabasePersistence(databaseTestFramework.GenericDatabaseInteractor().Accept(new GetConSurvDatabaseInteractorVisitor()), timeService, logger, databasePersistenceConfigurationMock.Object);


            databaseTestFramework.ResetDatabase();
            IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
            IConSurvDatabaseInteractor ConSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

            List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
            Assert.IsEmpty(tables1);

            IList<MigrationInstance> migrations = ConSurvDatabaseInteractor.GetAllMigrations();
            GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.ToList(), databaseInteractor);

            migrator.InitializeDatabaseAndMigrateIfRequired();

            return new PersistenceDisposable(result, new HashSet<IDisposable>() { databaseTestFramework, databaseInteractor });
        }
    }
}
