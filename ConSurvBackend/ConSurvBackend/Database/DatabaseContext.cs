using ConSurvBackend.Core.Configuration;
using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc.Migration;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ConSurvBackend.Core.Database
{
    public sealed class DatabaseContext : DbContext, IInitializable
    {

        private readonly IGeneralLogger _Logger;
        private readonly ITimeService _TimeService;
        private readonly IDatabaseManager _DatabaseManager;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _PersistedAPIServerConfiguration;
        public bool IsInitialized { get; private set; }
        public DbConnection Connection { get; private set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IGeneralLogger logger, ITimeService timeService, IDatabaseManager databaseManager, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> persistedAPIServerConfiguration) : base(options)
        {
            this._Logger = logger;
            this._TimeService = timeService;
            this._DatabaseManager = databaseManager;
            this._PersistedAPIServerConfiguration = persistedAPIServerConfiguration;
        }

        public void Initialize()
        {
            if (!IsInitialized)
            {
                IGenericDatabaseInteractor interactor = this._DatabaseManager.GetGenericDatabaseInteractor();
                Tools.ConnectToDatabaseWrapper(() =>
                {
                    this.Connection = this.Database.GetDbConnection();
                    this.Connection.Open();
                    GRYMigrator migrator = new GRYMigrator(this._Logger, this._TimeService, this.Connection, this._DatabaseManager.GetAllMigrations(), interactor);
                    migrator.InitializeDatabaseAndMigrateIfRequired();
                }, GeneralLogger.NoLog(), interactor.AdaptConnectionString(_PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseConnectionString));
                IsInitialized = true;
            }
        }
        public override void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
            }
            base.Dispose();
        }
    }
}
