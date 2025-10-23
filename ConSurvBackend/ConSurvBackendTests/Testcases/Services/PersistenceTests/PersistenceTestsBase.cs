using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    public abstract class PersistenceTestsBase
    {
        public PersistenceTestsBase()
        {
        }

        public abstract IPersistence GetPersistence();


        public abstract void PersistCameraTest();
        public void PersistCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using IPersistence persistence = this.GetPersistence();
                Camera testCamera = new Camera("id", "name");
                Assert.IsFalse(persistence.IsCamera(testCamera.Id));

                //act
                persistence.CreateCamera(testCamera);

                //assert
                Assert.IsTrue(persistence.IsCamera(testCamera.Id));
            }
        }

    }
}
