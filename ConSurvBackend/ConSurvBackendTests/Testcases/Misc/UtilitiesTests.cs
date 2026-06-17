using ConSurvBackend.Core.Misc;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ConSurvBackend.Tests.Testcases.Misc
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void GetVideoTargetFileTests()
        {
            // arrange
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>();
            string cameraId = "123456";
            bool inUTC = true;

            DateTime dateTime = new DateTime(2025, 06, 22, 13, 55, 09);
            timeServiceMock.Setup(timeService => timeService.GetCurrentTimeInUTCAsDateTimeOffset()).Returns(dateTime);
            string expectedName = $"{cameraId}_{dateTime.Year:D4}_{dateTime.Month:D2}_{dateTime.Day:D2}_{dateTime.Hour:D2}_{dateTime.Minute:D2}_{dateTime.Second:D2}.mp4";

            // act
            string actualName = Core.Misc.Utilities.GetVideoTargetFile(cameraId, inUTC, timeServiceMock.Object);

            // assert
            Assert.AreEqual(expectedName, actualName);
            timeServiceMock.Verify(timeService => timeService.GetCurrentTimeInUTCAsDateTimeOffset(), Times.Once);
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void GetVideoTargetFileTests_LocalTime()
        {
            // arrange
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>();
            string cameraId = "789";
            bool inUTC = false;

            DateTime dateTime = new DateTime(2025, 12, 31, 23, 59, 59);
            timeServiceMock.Setup(timeService => timeService.GetCurrentLocalTimeAsDateTimeOffset()).Returns(dateTime);
            string expectedName = $"{cameraId}_{dateTime.Year:D4}_{dateTime.Month:D2}_{dateTime.Day:D2}_{dateTime.Hour:D2}_{dateTime.Minute:D2}_{dateTime.Second:D2}.mp4";

            // act
            string actualName = Core.Misc.Utilities.GetVideoTargetFile(cameraId, inUTC, timeServiceMock.Object);

            // assert
            Assert.AreEqual(expectedName, actualName);
            timeServiceMock.Verify(timeService => timeService.GetCurrentLocalTimeAsDateTimeOffset(), Times.Once);
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void EscapeBasicAuthPasswords()
        {
            // arrange
            string input = $"test1: rtsp://user:password1@example.com/stream1 ; test2: rtsps://user:password2@example.com/stream2";
            string expectedOutput = "test1: rtsp://example.com/stream1 ; test2: rtsps://example.com/stream2";

            // act
            string actualOutput =Utilities.EscapeBasicAuthPasswords(input);

            // assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
