using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    public class PersistenceTransientTests : PersistenceTestsBase
    {
        public override IPersistence GetPersistence()
        {
            return TestUtilities.Utilities.GetTransientPersistence();
        }

        [TestMethod(nameof(AddCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void AddCameraTest()
        {
            this.AddCamera();
        }
    }
}
