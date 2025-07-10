using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data.Common;

namespace ConSurvBackend.Tests.TestUtilities
{
    public sealed class DatabaseTestFrameworkForMariaDB : DatabaseTestFrameworkTemplate
    {
        public DatabaseTestFrameworkForMariaDB() : base("consurvbackend_database", "Host=localhost; Port=3306;Username=user; Password=pa55w0rd; Database=ConSurvDatabase;", Utilities.GetTestMariaDBDatabaseFolder())
        {
        }


        public override void ConfigureDb<TDbContext>(DbContextOptionsBuilder<TDbContext> optionsBuilder)
        {
            optionsBuilder.UseMySql(this.ConnectionString, ServerVersion.AutoDetect(this.ConnectionString));
        }

        public override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public override string GetDatabaseName()
        {
            return "MariaDB";
        }
    }
}
