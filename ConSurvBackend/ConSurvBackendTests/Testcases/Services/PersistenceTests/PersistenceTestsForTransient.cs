using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    [TestClass]
    public class PersistenceTestsForTransient : PersistenceTestsBase
    {
        internal override PersistenceDisposable GetPersistence()
        {
            (TransientPersistence, ISet<IDisposable>) result = TestUtilities.Utilities.GetTransientPersistence();
            return new PersistenceDisposable(result.Item1, result.Item2);
        }

        [TestMethod(DisplayName =nameof(PersistenceTestsForTransient) + "." + nameof(CreateCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void CreateCameraTest()
        {
            this.CreateCamera();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForTransient) + "." + nameof(RemoveCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void RemoveCameraTest()
        {
            this.RemoveCamera();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForTransient) + "." + nameof(UpdateCameraTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void UpdateCameraTest()
        {
            this.UpdateCamera();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForTransient) + "." + nameof(GetAllCamerasTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void GetAllCamerasTest()
        {
            this.GetAllCameras();
        }

        [TestMethod(DisplayName = nameof(PersistenceTestsForTransient) + "." + nameof(ResetTest))]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public override void ResetTest()
        {
            this.Reset();
        }
    }
}
