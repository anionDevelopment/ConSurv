using ConSurvBackend.Core.BackgroundServices;
using ConSurvBackend.Core.Controller;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ConSurvBackend.Tests.Testcases.Controller
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
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IPreviewService> previewService = new Mock<IPreviewService>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            cameraServiceMock.Setup(mock => mock.CreateCamera("New camera", "rtsp://mycamera.example.com/stream")).Returns(cameraId);
            CameraController controller = new CameraController(GeneralLogger.NoLog(), persistence.Object, cameraServiceMock.Object, previewService.Object);

            // act
            IActionResult actualResult = controller.CreateCamera();

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(cameraId, (string)okObjectResult.Value);
            cameraServiceMock.Verify(mock => mock.CreateCamera("New camera", "rtsp://mycamera.example.com/stream"));
            cameraServiceMock.VerifyNoOtherCalls();
        }
    }
}
