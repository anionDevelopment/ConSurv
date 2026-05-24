using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCvSharp.ML;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    public class PersistenceTestsForMariaDB : PersistenceTestsBaseForDatabase
    {
        private DatabaseTestFrameworkForMariaDB? _DatabaseFramework;

        [TestInitialize]
        public void TestInitialize()
        {
            _DatabaseFramework = ConSurvBackend.Tests.TestUtilities.Utilities.GetDatabaseTestFrameworkForMariaDB();
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

        [TestMethod(DisplayName = nameof(PersistenceTestsForMariaDB) + "." + nameof(CreateCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void CreateCameraTest()
        {
            this.CreateCamera();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForMariaDB) + "." + nameof(RemoveCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void RemoveCameraTest()
        {
            this.RemoveCamera();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForMariaDB) + "." + nameof(UpdateCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void UpdateCameraTest()
        {
            this.UpdateCamera();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForMariaDB) + "." + nameof(GetAllCamerasTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void GetAllCamerasTest()
        {
            this.GetAllCameras();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForMariaDB) + "." + nameof(ResetTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void ResetTest()
        {
            this.Reset();
        }


    }
}
