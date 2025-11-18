using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    public abstract class PersistenceTestsBase
    {
        public PersistenceTestsBase()
        {
        }

        internal abstract PersistenceDisposable GetPersistence();

        public abstract void PersistCameraTest();
        public void PersistCamera()
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

    }
}
