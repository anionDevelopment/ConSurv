using ConSurvBackend.Core.Controller;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ConSurvBackend.Tests
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
            string cameraId = Guid.NewGuid().ToString();
            cameraServiceMock.Setup(mock => mock.CreateCamera(cameraDTO.Name, new Core.Model.RecordingModes.NoRecording())).Returns(cameraId);
            CameraController controller = new CameraController(GeneralLogger.NoLog(), persistence.Object, cameraServiceMock.Object);

            // act
            IActionResult actualResult = controller.CreateCamera(cameraDTO);

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(cameraId, (string)okObjectResult.Value);
            cameraServiceMock.Verify(mock => mock.CreateCamera(cameraDTO.Name, new Core.Model.RecordingModes.NoRecording()));
            cameraServiceMock.VerifyNoOtherCalls();
        }
    }
}
