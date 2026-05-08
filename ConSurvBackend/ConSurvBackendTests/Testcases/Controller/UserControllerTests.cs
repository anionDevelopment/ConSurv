using ConSurvBackend.Core.Controller;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IAuthenticationService = GRYLibrary.Core.APIServer.Services.Interfaces.IAuthenticationService;

namespace ConSurvBackend.Tests.Testcases.Controller
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Login_NullUser_ReturnsBadRequest()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object);

            // act
            IActionResult actualResult = controller.Login(null, "somepassword");

            // assert
            BadRequestObjectResult badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Login_NullPassword_ReturnsBadRequest()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object);

            // act
            IActionResult actualResult = controller.Login("someuser", null);

            // assert
            BadRequestObjectResult badRequestResult = actualResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TokenIsValid_ValidToken_ReturnsOkWithTrue()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            string accessToken = "valid-token-123";
            authServiceMock.Setup(mock => mock.AccessTokenIsValid(accessToken)).Returns(true);
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object);

            // act
            IActionResult actualResult = controller.TokenIsValid(accessToken);

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(true, okObjectResult.Value);
            authServiceMock.Verify(mock => mock.AccessTokenIsValid(accessToken), Times.Once);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TokenIsValid_InvalidToken_ReturnsOkWithFalse()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            Mock<ITimeService> timeServiceMock = new Mock<ITimeService>(MockBehavior.Strict);
            string accessToken = "expired-or-unknown-token";
            authServiceMock.Setup(mock => mock.AccessTokenIsValid(accessToken)).Returns(false);
            UserController controller = new UserController(ServerLog.GetTransientLog(), persistence.Object, authServiceMock.Object, timeServiceMock.Object);

            // act
            IActionResult actualResult = controller.TokenIsValid(accessToken);

            // assert
            OkObjectResult okObjectResult = actualResult as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.AreEqual(false, okObjectResult.Value);
            authServiceMock.Verify(mock => mock.AccessTokenIsValid(accessToken), Times.Once);
            authServiceMock.VerifyNoOtherCalls();
            timeServiceMock.VerifyNoOtherCalls();
        }
    }
}
