using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Misc.Migration;
using System.Collections.Generic;
using System.Reflection;

namespace ConSurvBackend.Core.Services
{
    public class DatabaseInteractorMariaDB : IConSurvDatabaseInteractor
    {
        private readonly MariaDBDatabaseInteractor _DatabaseInteractor;
        private readonly IList<MigrationInstance> _Migrations = GRYMigrator.LoadMigrationsFromResources(Assembly.GetExecutingAssembly(), "ConSurvBackend.Core.Resources.Database.MariaDB.Migrations.");
        public DatabaseInteractorMariaDB(IGenericDatabaseInteractor interactor)
        {
            this._DatabaseInteractor = (MariaDBDatabaseInteractor)interactor;
        }
        public IList<MigrationInstance> GetAllMigrations()
        {
            return this._Migrations;
        }

        public IGenericDatabaseInteractor GetGenericDatabaseInteractor()
        {
            return this._DatabaseInteractor;
        }

        public ISQLProvider GetSQLProvider()
        {
            return new SQLProviderMariaDB();
        }
    }
}
