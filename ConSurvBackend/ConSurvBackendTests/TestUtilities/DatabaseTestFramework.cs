using GRYLibrary.Core.APIServer.Utilities;

namespace ConSurvBackend.Tests.TestUtilities
{
    public sealed class DatabaseTestFramework : DatabaseTestFrameworkTemplate
    {
        public DatabaseTestFramework() : base("consurvbackend_database", "Server=localhost; Port=3306; Database=ConSurvBackendDatabase; Uid=user; Pwd=pa55w0rd;", Utilities.GetTestDatabaseFolder())
        {
        }
    }   
}
