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
            timeServiceMock.Setup(timeService => timeService.GetCurrentTimeInUTC()).Returns(dateTime);
            string expectedName = $"{dateTime.Year:D4}/{dateTime.Month:D2}/{dateTime.Day:D2}/{cameraId}_{dateTime.Year:D4}_{dateTime.Month:D2}_{dateTime.Day:D2}_{dateTime.Hour:D2}_{dateTime.Minute:D2}_{dateTime.Second:D2}.mp4";

            // act
            string actualName = Core.Misc.Utilities.GetVideoTargetFile(cameraId, inUTC, timeServiceMock.Object);

            // assert
            Assert.AreEqual(expectedName, actualName);
            timeServiceMock.Verify(timeService => timeService.GetCurrentTimeInUTC(), Times.Once);
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void EscapeBasicAuthPasswords()
        {
            // arrange
            string password1 = "password1";
            string password2 = "password2";
            string escapeString = "***";
            string input = $"test1: rtsp://user:{password1}@example.com/stream1 ; test2: rtsps://user:{password2}@example.com/stream1";
            string expectedOutput = input.Replace(password1, escapeString).Replace(password2, escapeString);

            // act
            string actualOutput =Utilities.EscapeBasicAuthPasswords(input);

            // assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
