using GRYLibrary.Core.Logging.GRYLogger;
using ConSurvBackend.Core.Miscellaneous;

namespace ConSurvBackend.Core.Services
{
    public class SQLProviderMariaDB : SQLProvider, ISQLProvider
    {
        public SQLProviderMariaDB(IGRYLog log) : base("MariaDB", log) { }

    }
}
