using GRYLibrary.Core.Logging.GRYLogger;
using ConSurvBackend.Core.Miscellaneous;

namespace ConSurvBackend.Core.Services
{
    public class SQLProviderPostgreSQL : SQLProvider, ISQLProvider
    {
        public SQLProviderPostgreSQL(IGRYLog log) : base("PostgreSQL", log) { }

    }
}
