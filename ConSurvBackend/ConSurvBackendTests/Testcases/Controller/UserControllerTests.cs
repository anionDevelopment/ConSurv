using ConSurvBackend.Core.Controller;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using IAuthenticationService = GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService;

namespace ConSurvBackend.Tests.Testcases.Controller
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.UnitTest))]
        public void Login_NullUser_ReturnsBadRequest()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            Mock<IBusinessLogicService> businessLogicServiceMock = new Mock<IBusinessLogicService>();
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object, businessLogicServiceMock.Object);

            // act
            IActionResult actualResult = controller.Login(null, "somepassword");

            // assert
            BadRequestObjectResult? badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.UnitTest))]
        public void Login_NullPassword_ReturnsBadRequest()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            Mock<IBusinessLogicService> businessLogicServiceMock = new Mock<IBusinessLogicService>();
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object, businessLogicServiceMock.Object);

            // act
            IActionResult actualResult = controller.Login("someuser", null);

            // assert
            BadRequestObjectResult? badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.UnitTest))]
        public void TokenIsValid_ValidToken_ReturnsOkWithTrue()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            string accessToken = "valid-token-123";
            authServiceMock.Setup(mock => mock.AccessTokenIsValid(accessToken)).Returns(true);
            Mock<IBusinessLogicService> businessLogicServiceMock = new Mock<IBusinessLogicService>();
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object, businessLogicServiceMock.Object);

            // act
            IActionResult actualResult = controller.TokenIsValid(accessToken);

            // assert
            OkObjectResult? okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(true, okObjectResult.Value);
            authServiceMock.Verify(mock => mock.AccessTokenIsValid(accessToken), Times.Once);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.UnitTest))]
        public void TokenIsValid_InvalidToken_ReturnsOkWithFalse()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            string accessToken = "expired-or-unknown-token";
            authServiceMock.Setup(mock => mock.AccessTokenIsValid(accessToken)).Returns(false);
            Mock<IBusinessLogicService> businessLogicServiceMock = new Mock<IBusinessLogicService>();
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object, businessLogicServiceMock.Object);

            // act
            IActionResult actualResult = controller.TokenIsValid(accessToken);

            // assert
            OkObjectResult? okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(false, okObjectResult.Value);
            authServiceMock.Verify(mock => mock.AccessTokenIsValid(accessToken), Times.Once);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.UnitTest))]
        public void TokenIsValid_DoesNotWriteTheTokenToTheLog()
        {
            // Regression-test for finding CSV-31: the access-token must never appear in the log.
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            Mock<IBusinessLogicService> businessLogicServiceMock = new Mock<IBusinessLogicService>();
            string accessToken = "super-secret-access-token-value";
            authServiceMock.Setup(mock => mock.AccessTokenIsValid(accessToken)).Returns(true);
            List<string> loggedMessages = new List<string>();
            Mock<IGRYLog> loggerMock = new Mock<IGRYLog>();
            loggerMock.Setup(logger => logger.Log(It.IsAny<string>())).Callback<string>(message => loggedMessages.Add(message));
            Mock<IServerLog> serverLogMock = new Mock<IServerLog>();
            serverLogMock.Setup(serverLog => serverLog.Logger).Returns(loggerMock.Object);
            UserController controller = new UserController(serverLogMock.Object, persistence.Object, authServiceMock.Object, timeServiceMock.Object, businessLogicServiceMock.Object);

            // act
            controller.TokenIsValid(accessToken);

            // assert
            foreach (string message in loggedMessages)
            {
                Assert.IsFalse(message.Contains(accessToken), $"The log-message \"{message}\" contains the access-token.");
            }
        }
    }
}
