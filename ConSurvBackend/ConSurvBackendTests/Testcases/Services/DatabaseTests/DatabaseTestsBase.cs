using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace ConSurvBackend.Tests.Testcases.Services.DatabaseTests
{
    public abstract class DatabaseTestsBase
    {
        protected abstract DatabaseTestFrameworkTemplate GetDatabaseTestFrameworkImplementation();
        protected DatabaseTestFrameworkTemplate GetDatabaseTestFramework(bool runMigrations)
        {
            DatabaseTestFrameworkTemplate result = this.GetDatabaseTestFrameworkImplementation();
            this.PrepareDatabase(result, runMigrations);
            return result;
        }
        /// <summary>
        /// Resets database.
        /// If desired, all available migrations will be done.
        /// </summary>
        private void PrepareDatabase(DatabaseTestFrameworkTemplate databaseTestFramework, bool runMigrations)
        {
            databaseTestFramework.ResetDatabase();
            IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
            IConSurvDatabaseInteractor conSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

            List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
            Assert.IsEmpty(tables1);

            if (runMigrations)
            {
                IList<MigrationInstance> migrations = conSurvDatabaseInteractor.GetAllMigrations();
                Assert.IsNotEmpty(migrations);
                GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.ToList(), databaseInteractor);
                migrator.InitializeDatabaseAndMigrateIfRequired();
            }
        }
        public abstract void Migration000001Test();
        public void Migration000001()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using (DatabaseTestFrameworkTemplate databaseTestFramework = this.GetDatabaseTestFramework(false))
                {
                    IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
                    IConSurvDatabaseInteractor ConSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

                    List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsEmpty(tables1);

                    IList<MigrationInstance> migrations = ConSurvDatabaseInteractor.GetAllMigrations();
                    GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.Take(1).ToList(), databaseInteractor);

                    //act
                    migrator.InitializeDatabaseAndMigrateIfRequired();

                    //assert
                    Assert.HasCount(1, migrator.GetExecutedMigrations());
                    Assert.AreEqual("Migration000001", migrator.GetExecutedMigrations().First().MigrationName);

                    List<string> tables2 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsLessThan(tables2.Count, 1);
                    //TODO add more migration-specific assertions
                }
            }
        }


        public abstract void AllMigrationsAreWorkingTest();
        public void AllMigrationsAreWorking()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                using (DatabaseTestFrameworkTemplate databaseTestFramework = this.GetDatabaseTestFramework(false))
                {
                    IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
                    IConSurvDatabaseInteractor ConSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

                    List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsEmpty(tables1);

                    IList<MigrationInstance> migrations = ConSurvDatabaseInteractor.GetAllMigrations();
                    GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.ToList(), databaseInteractor);

                    //act
                    migrator.InitializeDatabaseAndMigrateIfRequired();

                    //assert
                    Assert.HasCount(migrations.Count, migrator.GetExecutedMigrations());

                    List<string> tables2 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsLessThan(tables2.Count, 1);
                }
            }
        }

        public abstract void LoadCameraTest();
        public void LoadCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                using (DatabaseTestFrameworkTemplate databaseTestFramework = this.GetDatabaseTestFramework(true))
                {
                    //arrange
                    using IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
                    IConSurvDatabaseInteractor conSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());
                    ITimeService timeService = new TimeService();
                    IGRYLog log = GRYLog.Create();
                    Mock<IDatabasePersistenceConfiguration> databasePersistenceConfigurationMock = new Mock<IDatabasePersistenceConfiguration>(MockBehavior.Strict);
                    databasePersistenceConfigurationMock.SetupGet(m => m.DatabaseConnectionString).Returns(databaseTestFramework.ConnectionString);
                    using DatabasePersistence databasePersistence = new DatabasePersistence(conSurvDatabaseInteractor, timeService, log, databasePersistenceConfigurationMock.Object);
                    ConSurvBackend.Core.Model.Base.Camera expectedCamera = new ConSurvBackend.Core.Model.Base.Camera("id", "name");
                    databasePersistence.CreateCamera(expectedCamera);

                    //act
                    IDictionary<string, Camera> cameras = databasePersistence.GetAllCameras();

                    //assert
                    Assert.HasCount(1, cameras);
                    Assert.IsTrue(cameras.ContainsKey(expectedCamera.Id));
                    Assert.AreEqual(expectedCamera, cameras[expectedCamera.Id]);
                }
            }
        }
    }
}
