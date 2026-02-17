using GRYLibrary.Core.APIServer.Services.Database;

namespace ConSurvBackend.Core.Services
{
    public interface IConSurvDatabaseInteractor : IProjectSpecificDatabaseInteractor
    {
        string Id { get; }
        public ISQLProvider GetSQLProvider();
    }
}
