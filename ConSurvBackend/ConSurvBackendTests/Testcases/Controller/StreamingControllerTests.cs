using ConSurvBackend.Core.Controller;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;

namespace ConSurvBackend.Tests.Testcases.Controller
{
    [TestClass]
    public class StreamingControllerTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Stream_FilenameWithPathTraversal_ReturnsBadRequest()
        {
            // arrange
            Mock<IApplicationConstants> applicationConstantsMock = new Mock<IApplicationConstants>(MockBehavior.Strict);
            StreamingController controller = new StreamingController(applicationConstantsMock.Object, ServerLog.GetTransientLog());

            // act
            IActionResult actualResult = controller.Stream("cam1", "../../etc/passwd");

            // assert
            BadRequestObjectResult badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            applicationConstantsMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Stream_FilenameWithSpaces_ReturnsBadRequest()
        {
            // arrange
            Mock<IApplicationConstants> applicationConstantsMock = new Mock<IApplicationConstants>(MockBehavior.Strict);
            StreamingController controller = new StreamingController(applicationConstantsMock.Object, ServerLog.GetTransientLog());

            // act
            IActionResult actualResult = controller.Stream("cam1", "invalid file.m3u8");

            // assert
            BadRequestObjectResult badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            applicationConstantsMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Stream_FilenameWithSpecialChars_ReturnsBadRequest()
        {
            // arrange
            Mock<IApplicationConstants> applicationConstantsMock = new Mock<IApplicationConstants>(MockBehavior.Strict);
            StreamingController controller = new StreamingController(applicationConstantsMock.Object, ServerLog.GetTransientLog());

            // act
            IActionResult actualResult = controller.Stream("cam1", "file;rm-rf.m3u8");

            // assert
            BadRequestObjectResult badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            applicationConstantsMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Stream_ValidFilenamePattern_FileNotFound_ReturnsNotFound()
        {
            // arrange
            string nonExistentDataFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Mock<IApplicationConstants> applicationConstantsMock = new Mock<IApplicationConstants>(MockBehavior.Strict);
            applicationConstantsMock.Setup(mock => mock.GetDataFolder()).Returns(nonExistentDataFolder);
            StreamingController controller = new StreamingController(applicationConstantsMock.Object, ServerLog.GetTransientLog());

            // act
            IActionResult actualResult = controller.Stream("cam1", "stream.m3u8");

            // assert
            Assert.IsInstanceOfType(actualResult, typeof(NotFoundResult));
            applicationConstantsMock.Verify(mock => mock.GetDataFolder(), Times.Once);
            applicationConstantsMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Stream_ValidTsFilenamePattern_FileNotFound_ReturnsNotFound()
        {
            // arrange
            string nonExistentDataFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Mock<IApplicationConstants> applicationConstantsMock = new Mock<IApplicationConstants>(MockBehavior.Strict);
            applicationConstantsMock.Setup(mock => mock.GetDataFolder()).Returns(nonExistentDataFolder);
            StreamingController controller = new StreamingController(applicationConstantsMock.Object, ServerLog.GetTransientLog());

            // act
            IActionResult actualResult = controller.Stream("cam1", "segment_001.ts");

            // act & assert
            Assert.IsInstanceOfType(actualResult, typeof(NotFoundResult));
            applicationConstantsMock.Verify(mock => mock.GetDataFolder(), Times.Once);
            applicationConstantsMock.VerifyNoOtherCalls();
        }
    }
}
