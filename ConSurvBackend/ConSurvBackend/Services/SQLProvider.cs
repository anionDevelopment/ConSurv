using GRYLibrary.Core.Misc;

namespace ConSurvBackend.Core.Services
{
    public class SQLProvider : AbstractSQLProvider, ISQLProvider
    {
        public SQLProvider() : base("ConSurvBackend.Core.Database.SQL") { }

    }
}
