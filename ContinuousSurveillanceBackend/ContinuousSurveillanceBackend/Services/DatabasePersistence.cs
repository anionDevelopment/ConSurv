using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using GRYLibrary.Core.Miscellaneous.Migration;
using System.Linq;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Exceptions;
using System.Data.SqlClient;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using System.IO;
using System.Reflection;
using ContinuousSurveillanceBackend.Core.Database.Contexts;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public class DatabasePersistence : IPersistence
    {
        private readonly DatabaseContext _DatabaseContext;
        private readonly IGeneralLogger _Logger;
        private readonly ITimeService _TimeService;
        public DatabasePersistence(IGeneralLogger logger, ITimeService timeService, DbContextOptions<DatabaseContext> options)
        {
            this._Logger = logger;
            this._TimeService = timeService;
            this._DatabaseContext = new DatabaseContext(options);
            if (this.IsAvailable())
            {
                this.Initialize();
            }
            else
            {
                throw new DependencyNotAvailableException("Database not available.");
            }
        }

        private void Initialize()
        {
            var assembly = Assembly.GetEntryAssembly();
            IList<MigrationInstance> instances = new List<MigrationInstance>();
            foreach (var manifestResourceName in assembly.GetManifestResourceNames())
            {
                string resourcePrefix = "ContinuousSurveillanceBackend.Resources.Database.Migrations.";
                if (manifestResourceName.StartsWith(resourcePrefix))
                {
                    using Stream stream = assembly.GetManifestResourceStream(manifestResourceName);
                    using StreamReader reader = new StreamReader(stream);
                    string migrationContent = reader.ReadToEnd();
                    string name = manifestResourceName[..resourcePrefix.Length][(manifestResourceName.Length - 4)..];
                    instances.Add(new MigrationInstance() { MigrationName = name, MigrationContent = migrationContent });
                }
            }
            instances = instances.OrderBy(instance => instance.MigrationName).ToList();
            using SqlConnection connection = (SqlConnection)this._DatabaseContext.Database.GetDbConnection();
            connection.Open();
            GRYMigrator migrator = new GRYMigrator(this._Logger, _TimeService, connection, instances);
            migrator.InitializeDatabaseAndMigrateIfRequired();
        }
        public void SetCustomer(Database.Contexts.DataTypes.Customer customer)
        {
            this._DatabaseContext.Customer.Add(customer);
            this._DatabaseContext.SaveChanges();
        }

        public ISet<Database.Contexts.DataTypes.Customer> GetAllCustomer()
        {
            return this._DatabaseContext.Customer.ToHashSet();
        }

        public Database.Contexts.DataTypes.Customer GetCustomer(int customerId)
        {
            return this._DatabaseContext.Customer.First(c => c.Id == customerId);
        }

        public void RemoveCustomer(int customerId)
        {
            this._DatabaseContext.Remove(this.GetCustomer(customerId));
            this._DatabaseContext.SaveChanges();
        }

        public bool IsAvailable()
        {
            return this._DatabaseContext.Database.CanConnect();
        }
    }
}
