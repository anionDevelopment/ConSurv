using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc.Migration;
using System.Collections.Generic;
using System.Reflection;

namespace ConSurvBackend.Core.Services
{
    public class DatabaseInteractorPostgreSQL : IConSurvDatabaseInteractor
    {
        public IGRYLog Log
        {
            get
            {
                return this._DatabaseInteractor.Log;
            }
        }
        private readonly PostgreSQLDatabaseInteractor _DatabaseInteractor;
        private readonly IList<MigrationInstance> _Migrations = GRYMigrator.LoadMigrationsFromResources(Assembly.GetExecutingAssembly(), "ConSurvBackend.Core.Resources.Database.PostgreSQL.Migrations.");
        public DatabaseInteractorPostgreSQL(IGenericDatabaseInteractor interactor)
        {
            this._DatabaseInteractor = (PostgreSQLDatabaseInteractor)interactor;
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
            return new SQLProviderPostgreSQL();
        }

    }
}
