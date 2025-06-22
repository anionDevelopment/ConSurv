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
    }
}
