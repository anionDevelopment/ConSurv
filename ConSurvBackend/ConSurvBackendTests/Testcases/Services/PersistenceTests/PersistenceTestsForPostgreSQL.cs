using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    [Ignore("this file causes problems on some systems due test-case-runs which do not terminate.")]
    public class PersistenceTestsForPostgreSQL : PersistenceTestsBaseForDatabase
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
