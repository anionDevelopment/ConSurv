using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Services;
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
            IConSurvDatabaseInteractor openDMSDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

            List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
            Assert.IsEmpty(tables1);

            if (runMigrations)
            {
                IList<MigrationInstance> migrations = openDMSDatabaseInteractor.GetAllMigrations();
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
                    databaseTestFramework.ResetDatabase();
                    IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
                    IConSurvDatabaseInteractor ConSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

                    List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsEmpty(tables1);

                    IList<MigrationInstance> migrations = ConSurvDatabaseInteractor.GetAllMigrations();
                    GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.Take(1).ToList(), databaseInteractor);

                    //act
                    migrator.InitializeDatabaseAndMigrateIfRequired();

                    //assert
                    Assert.AreEqual(1, migrator.GetExecutedMigrations().Count);
                    Assert.AreEqual("Migration000001", migrator.GetExecutedMigrations().First().MigrationName);

                    List<string> tables2 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsTrue(1 < tables2.Count);
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
                    databaseTestFramework.ResetDatabase();
                    IGenericDatabaseInteractor databaseInteractor = databaseTestFramework.GenericDatabaseInteractor();
                    IConSurvDatabaseInteractor ConSurvDatabaseInteractor = databaseInteractor.Accept(new GetConSurvDatabaseInteractorVisitor());

                    List<string> tables1 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsEmpty(tables1);

                    IList<MigrationInstance> migrations = ConSurvDatabaseInteractor.GetAllMigrations();
                    GRYMigrator migrator = new GRYMigrator(new TimeService(), migrations.ToList(), databaseInteractor);

                    //act
                    migrator.InitializeDatabaseAndMigrateIfRequired();

                    //assert
                    Assert.AreEqual(migrations.Count, migrator.GetExecutedMigrations().Count);

                    List<string> tables2 = databaseInteractor.GetAllTableNames().ToList();
                    Assert.IsTrue(1 < tables2.Count);
                }
            }
        }
    }
}
