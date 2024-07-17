using ContinuousSurveillanceBackend.Core.Controller;
using ContinuousSurveillanceBackend.Core.Model.DTOs;
using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ContinuousSurveillanceBackend.Tests
{
    [TestClass]
    public class CameraControllerTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestSomeFunction()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<ICameraService> cameraService= new Mock<ICameraService>();
            CameraController controller = new CameraController( GeneralLogger.NoLog(), persistence.Object, cameraService.Object);
            CreateCameraDTO cameraDTO = new CreateCameraDTO();

            // act
            IActionResult actualResult = controller.CreateCamera(cameraDTO);

            // assert
            Assert.IsTrue(actualResult is OkObjectResult);
            decimal acturalResultValue = (decimal)(actualResult as OkObjectResult).Value;
        }
    }
}
