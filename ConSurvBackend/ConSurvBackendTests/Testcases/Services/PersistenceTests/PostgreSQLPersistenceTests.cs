using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    [Ignore]
    public class PostgreSQLPersistenceTests : PersistenceDatabaseTestsBase
    {
        protected override DatabaseTestFrameworkTemplate GetDatabaseTestFramework()
        {
            return ConSurvBackend.Tests.TestUtilities.Utilities.GetDatabaseTestFrameworkForPostgreSQL();
        }

        [TestMethod(nameof(PersistCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void PersistCameraTest()
        {
            this.PersistCamera();
        }

    }
}
