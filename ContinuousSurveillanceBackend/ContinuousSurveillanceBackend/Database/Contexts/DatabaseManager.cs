using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Misc.Migration;
using System.Collections.Generic;
using System.Reflection;

namespace ContinuousSurveillanceBackend.Core.Database.Contexts
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly MariaDBDatabaseInteractor _MariaDBDatabaseInteractor = new MariaDBDatabaseInteractor();
        private readonly IList<MigrationInstance> _Migrations = GRYMigrator.LoadMigrationsFromResources(Assembly.GetExecutingAssembly(), "ContinuousSurveillanceBackend.Core.Database.Migrations.");
        public IList<MigrationInstance> GetAllMigrations()
        {
            return this._Migrations;
        }

        public IGenericDatabaseInteractor GetGenericDatabaseInteractor()
        {
            return this._MariaDBDatabaseInteractor;
        }
    }
}
