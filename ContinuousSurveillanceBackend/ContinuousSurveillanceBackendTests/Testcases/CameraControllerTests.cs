using ContinuousSurveillanceBackend.Core.Controller;
using ContinuousSurveillanceBackend.Core.Model.DTOs;
using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc;
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
            Mock<ICameraService> cameraServiceMock = new Mock<ICameraService>(MockBehavior.Strict);
            CreateCameraDTO cameraDTO = new CreateCameraDTO()
            {
                Name = "MyCamera",
            };
            cameraServiceMock.Setup(mock => mock.CreateCamera(cameraDTO.Name, new Core.Model.RecordingModes.NoRecording()));
            CameraController controller = new CameraController(GeneralLogger.NoLog(), persistence.Object, cameraServiceMock.Object);

            // act
            IActionResult actualResult = controller.CreateCamera(cameraDTO);

            // assert
            Assert.IsTrue(actualResult is OkResult);
            cameraServiceMock.Verify(mock => mock.CreateCamera(cameraDTO.Name, new Core.Model.RecordingModes.NoRecording()));
            cameraServiceMock.VerifyNoOtherCalls();
        }
    }
}
