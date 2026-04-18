using GRYLibrary.Core.APIServer.Services.Database;

namespace ConSurvBackend.Core.Services
{
    public interface IConSurvDatabaseInteractor : IProjectSpecificDatabaseInteractor
    {
        /// <summary>
        /// Gets the unique identifier of this database interactor instance, mirrored from the underlying generic database interactor.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Returns the SQL provider that supplies database-engine-specific SQL scripts for this interactor.
        /// </summary>
        /// <returns>The <see cref="ISQLProvider"/> matching the underlying database engine.</returns>
        public ISQLProvider GetSQLProvider();
    }
}
