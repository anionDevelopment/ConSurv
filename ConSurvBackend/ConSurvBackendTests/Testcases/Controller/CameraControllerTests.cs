using ConSurvBackend.Core.Controller;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.Testcases.Controller
{
    [TestClass]
    public class CameraControllerTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestCreateCamera()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            cameraServiceMock.Setup(mock => mock.CreateCamera("New camera", "rtsp://mycamera.example.com/stream")).Returns(cameraId);
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.CreateCamera();

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(cameraId, (string)okObjectResult.Value);
            cameraServiceMock.Verify(mock => mock.CreateCamera("New camera", "rtsp://mycamera.example.com/stream"));
            cameraServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestRemoveCamera()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            cameraServiceMock.Setup(mock => mock.RemoveCamera(cameraId));
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.RemoveCamera(cameraId);

            // assert
            OkResult okResult = actualResult as OkResult;
            Assert.IsNotNull(okResult);
            cameraServiceMock.Verify(mock => mock.RemoveCamera(cameraId), Times.Once);
            cameraServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestUpdateCamera()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            UpdateCameraDTO updateCameraDTO = new UpdateCameraDTO
            {
                CameraId = cameraId,
                Name = "Updated camera",
                VideoInformationDTO = new VideoInformationDTO
                {
                    StreamURL = "rtsp://updated.example.com/stream",
                    SupportsPTZViaONVIF = false,
                },
                RecordModeDTO = new RecordModeDTO { RecordMode = nameof(RecordAlways) },
            };
            cameraServiceMock.Setup(mock => mock.UpdateCamera(It.Is<Camera>(c => c.Id == cameraId)));
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.UpdateCamera(updateCameraDTO);

            // assert
            OkResult okResult = actualResult as OkResult;
            Assert.IsNotNull(okResult);
            cameraServiceMock.Verify(mock => mock.UpdateCamera(It.Is<Camera>(c => c.Id == cameraId)), Times.Once);
            cameraServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestGetCameraById()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            Camera camera = new Camera(cameraId, "Test camera");
            CameraDTO cameraDTO = new CameraDTO { CameraId = cameraId, Name = "Test camera" };
            cameraServiceMock.Setup(mock => mock.GetCameraById(cameraId)).Returns(camera);
            cameraServiceMock.Setup(mock => mock.ToDTO(camera)).Returns(cameraDTO);
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.Camera(cameraId);

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(cameraDTO, okObjectResult.Value);
            cameraServiceMock.Verify(mock => mock.GetCameraById(cameraId), Times.Once);
            cameraServiceMock.Verify(mock => mock.ToDTO(camera), Times.Once);
            cameraServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestGetAllCameras()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            Camera camera = new Camera(cameraId, "Test camera");
            CameraDTO cameraDTO = new CameraDTO { CameraId = cameraId, Name = "Test camera" };
            IDictionary<string, Camera> cameras = new Dictionary<string, Camera> { { cameraId, camera } };
            cameraServiceMock.Setup(mock => mock.GetAllCameras()).Returns(cameras);
            cameraServiceMock.Setup(mock => mock.ToDTO(camera)).Returns(cameraDTO);
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.Cameras();

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            List<CameraDTO> resultList = okObjectResult.Value as List<CameraDTO>;
            Assert.IsNotNull(resultList);
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(cameraDTO, resultList[0]);
            cameraServiceMock.Verify(mock => mock.GetAllCameras(), Times.Once);
            cameraServiceMock.Verify(mock => mock.ToDTO(camera), Times.Once);
            cameraServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestListVideos()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            IDictionary<string, IList<string>> videos = new Dictionary<string, IList<string>>
            {
                { cameraId, new List<string> { "video1.mp4", "video2.mp4" } }
            };
            cameraServiceMock.Setup(mock => mock.GetVideos()).Returns(videos);
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.ListVideos();

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(videos, okObjectResult.Value);
            cameraServiceMock.Verify(mock => mock.GetVideos(), Times.Once);
            cameraServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestRemoveVideo()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IBusinessLogicService> cameraServiceMock = new Mock<IBusinessLogicService>(MockBehavior.Strict);
            Mock<IRuntimeData> runtimeData = new Mock<IRuntimeData>(MockBehavior.Strict);
            string cameraId = Guid.NewGuid().ToString();
            string filename = "video1.mp4";
            cameraServiceMock.Setup(mock => mock.RemoveVideo(cameraId, filename));
            CameraController controller = new CameraController(ServerLog.GetTransientLog(), persistence.Object, cameraServiceMock.Object, runtimeData.Object);

            // act
            IActionResult actualResult = controller.RemoveVideo(cameraId, filename);

            // assert
            OkResult okResult = actualResult as OkResult;
            Assert.IsNotNull(okResult);
            cameraServiceMock.Verify(mock => mock.RemoveVideo(cameraId, filename), Times.Once);
            cameraServiceMock.VerifyNoOtherCalls();
        }
    }
}
