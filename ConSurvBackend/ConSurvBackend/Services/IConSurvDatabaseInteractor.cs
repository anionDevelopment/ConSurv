using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.Misc.Migration;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IConSurvDatabaseInteractor 
    {
        public IGenericDatabaseInteractor GetGenericDatabaseInteractor();
        public IList<MigrationInstance> GetAllMigrations();
        public ISQLProvider GetSQLProvider();
    }
}
