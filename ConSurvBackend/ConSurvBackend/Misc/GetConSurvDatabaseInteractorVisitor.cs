using GRYLibrary.Core.APIServer.Services.Database;
using ConSurvBackend.Core.Services;
using System;

namespace ConSurvBackend.Core.Misc
{
    public class GetConSurvDatabaseInteractorVisitor : IGenericDatabaseInteractorVisitor<IConSurvDatabaseInteractor>
    {

        public IConSurvDatabaseInteractor Handle(MariaDBDatabaseInteractor mariaDBDatabaseInteractor)
        {
            return new DatabaseInteractorMariaDB(mariaDBDatabaseInteractor);
        }

        public IConSurvDatabaseInteractor Handle(OracleDatabaseInteractor oracleDatabaseInteractor)
        {
            throw new NotSupportedException();
        }

        public IConSurvDatabaseInteractor Handle(SQLServerDatabaseInteractor sQLServerDatabaseInteractor)
        {
            throw new NotSupportedException();
        }

        public IConSurvDatabaseInteractor Handle(PostgreSQLDatabaseInteractor postgreSQLDatabaseInteractor)
        {
            return new DatabaseInteractorPostgreSQL(postgreSQLDatabaseInteractor);
        }
    }
}
