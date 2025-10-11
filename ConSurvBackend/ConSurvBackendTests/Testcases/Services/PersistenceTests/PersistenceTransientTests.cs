using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Services;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    public class PersistenceTransientTests : PersistenceTestsBase
    {
        public override IPersistence GetPersistence()
        {
            return TestUtilities.Utilities.GetTransientPersistence();
        }



        [TestMethod(nameof(PersistCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void PersistCameraTest()
        {
            this.PersistCamera();
        }
    }
}
