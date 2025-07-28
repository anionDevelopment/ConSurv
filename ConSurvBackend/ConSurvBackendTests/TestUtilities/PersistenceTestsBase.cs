using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ConSurvBackend.Tests.TestUtilities
{
    public abstract class PersistenceTestsBase
    {
        public PersistenceTestsBase()
        {
        }

        public abstract IPersistence GetPersistence();

        public abstract void AddCameraTest();
        public void AddCamera()
        {
            //arrange
            using IPersistence persistence = this.GetPersistence();
            Camera testCamera = new Camera("ABCDEF", "Camera1")
            {
                VideoInformation = new VideoInformation()
                {
                    Certificate = null,
                    IsONVIFCamera = false,
                    StreamURL = "rtsp://192.168.1.10/stream"
                },
            };
            Assert.IsFalse(persistence.IsCamera(testCamera.Id));

            //act
            persistence.CreateCamera(testCamera);

            //assert
            Assert.IsTrue(persistence.IsCamera(testCamera.Id));
            var camerasWithCorrectId = persistence.GetAllCameras().Values.Where(c => c.Id == testCamera.Id);
            Assert.ContainsSingle(camerasWithCorrectId);
            var reloadedCamera = camerasWithCorrectId.First();
            Assert.AreEqual(testCamera, reloadedCamera);
        }
    }
}
