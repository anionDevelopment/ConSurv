using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    [Ignore("this file causes problems on some systems due test-case-runs which do not terminate.")]
    public class PersistenceTestsForMariaDB : PersistenceTestsBaseForDatabase
    {
        protected override DatabaseTestFrameworkTemplate GetDatabaseTestFramework()
        {
            return ConSurvBackend.Tests.TestUtilities.Utilities.GetDatabaseTestFrameworkForMariaDB();
        }

        [TestMethod(nameof(PersistCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void PersistCameraTest()
        {
            this.PersistCamera();
        }


    }
}
