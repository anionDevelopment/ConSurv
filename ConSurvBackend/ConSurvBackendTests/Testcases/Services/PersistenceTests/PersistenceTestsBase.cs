using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    public abstract class PersistenceTestsBase
    {
        public PersistenceTestsBase()
        {
        }

        internal abstract PersistenceDisposable GetPersistence();

        public abstract void CreateCameraTest();
        public void CreateCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                Camera testCamera = new Camera("id", "name");
                Assert.IsFalse(persistenceD.Persistence.IsCamera(testCamera.Id));

                //act
                persistenceD.Persistence.CreateCamera(testCamera);

                //assert
                Assert.IsTrue(persistenceD.Persistence.IsCamera(testCamera.Id));
            }
        }

        public abstract void RemoveCameraTest();
        public void RemoveCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                Camera testCamera = new Camera("id", "name");
                persistence.CreateCamera(testCamera);
                Assert.IsTrue(persistence.IsCamera(testCamera.Id));

                //act
                persistence.RemoveCamera(testCamera.Id);

                //assert
                Assert.IsFalse(persistence.IsCamera(testCamera.Id));
            }
        }

        public abstract void UpdateCameraTest();
        public void UpdateCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                Camera originalCamera = new Camera("id", "original-name");
                persistence.CreateCamera(originalCamera);
                Camera updatedCamera = new Camera("id", "updated-name");

                //act
                persistence.UpdateCamera(updatedCamera);

                //assert
                IDictionary<string, Camera> allCameras = persistence.GetAllCameras();
                Assert.IsTrue(allCameras.ContainsKey("id"));
                Assert.AreEqual("updated-name", allCameras["id"].Name);
            }
        }

        public abstract void GetAllCamerasTest();
        public void GetAllCameras()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                IDictionary<string, Camera> empty = persistence.GetAllCameras();
                Assert.AreEqual(0, empty.Count);
                Camera firstCamera = new Camera("id1", "first");
                Camera secondCamera = new Camera("id2", "second");
                persistence.CreateCamera(firstCamera);
                persistence.CreateCamera(secondCamera);

                //act
                IDictionary<string, Camera> allCameras = persistence.GetAllCameras();

                //assert
                Assert.AreEqual(2, allCameras.Count);
                Assert.IsTrue(allCameras.ContainsKey("id1"));
                Assert.IsTrue(allCameras.ContainsKey("id2"));
                Assert.AreEqual("first", allCameras["id1"].Name);
                Assert.AreEqual("second", allCameras["id2"].Name);
            }
        }

        public abstract void ResetTest();
        public void Reset()
        {
            // Note: post-Reset state differs between backends. TransientPersistence.Reset clears
            // the camera dictionary; DatabasePersistence.Reset drops the schema, so a subsequent
            // IsCamera/GetAllCameras call requires fresh migrations. The shared assertion is
            // therefore limited to "Reset must execute without throwing".
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                persistence.CreateCamera(new Camera("id1", "first"));
                persistence.CreateCamera(new Camera("id2", "second"));

                //act & assert: must not throw
                persistence.Reset();
            }
        }

    }
}
