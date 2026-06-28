using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCvSharp.ML;

namespace ConSurvBackend.Tests.Testcases.Services.DatabaseTests
{
    [TestClass]
    public class DatabaseTestsForPostgresSQL : DatabaseTestsBase
    {

        private DatabaseTestFrameworkForPostgreSQL? _DatabaseFramework;

        [TestInitialize]
        public void TestInitialize()
        {
            _DatabaseFramework = ConSurvBackend.Tests.TestUtilities.Utilities.GetDatabaseTestFrameworkForPostgreSQL();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _DatabaseFramework?.Dispose();
            _DatabaseFramework = null;
        }

        protected override DatabaseTestFrameworkTemplate GetDatabaseTestFramework()
        {
            return _DatabaseFramework!;
        }

        [TestMethod(nameof(Migration000001Test))]
        [TestProperty(nameof(TestKind), nameof(TestKind.IntegrationTest))]
        public override void Migration000001Test()
        {
            this.Migration000001();
        }

        [TestMethod(nameof(AllMigrationsAreWorkingTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.IntegrationTest))]
        public override void AllMigrationsAreWorkingTest()
        {
            this.AllMigrationsAreWorking();
        }
        [TestMethod(nameof(LoadCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.IntegrationTest))]
        public override void LoadCameraTest()
        {
            this.LoadCamera();
        }
    }
}
