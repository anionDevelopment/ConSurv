using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Misc.Migration;
using System.Collections.Generic;
using System.Reflection;

namespace ConSurvBackend.Core.Services
{
    public class DatabaseInteractorPostgreSQL : IConSurvDatabaseInteractor
    {
        public string Id { get; private set; }
        private readonly PostgreSQLDatabaseInteractor _DatabaseInteractor;
        private readonly IList<MigrationInstance> _Migrations = GRYMigrator.LoadMigrationsFromResources(Assembly.GetExecutingAssembly(), "ConSurvBackend.Core.Resources.Database.PostgreSQL.Migrations.");
        public DatabaseInteractorPostgreSQL(IGenericDatabaseInteractor interactor)
        {
            this._DatabaseInteractor = (PostgreSQLDatabaseInteractor)interactor;
            this.Id = this._DatabaseInteractor.Id;
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
        public void SetLogConnectionAttemptErrors(bool enabled)
        {
            this._DatabaseInteractor.SetLogConnectionAttemptErrors(enabled);
        }

    }
}
