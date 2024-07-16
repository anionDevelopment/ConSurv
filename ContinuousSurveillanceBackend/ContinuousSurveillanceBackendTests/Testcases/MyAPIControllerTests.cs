using ContinuousSurveillanceBackend.Core.Controller;
using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ContinuousSurveillanceBackend.Tests
{
    [TestClass]
    public class MyAPIControllerTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void TestSomeFunction()
        {
            // arrange
            Mock<IPersistence> persistence = new Mock<IPersistence>();
            ExampleController controller = new ExampleController( GeneralLogger.NoLog(), persistence.Object);
            decimal parameter1 = 2.5m;
            decimal parameter2 = 3m;
            decimal expectedResultValue = 5.5m;

            // act
            IActionResult actualResult = controller.Calculate("Add", parameter1, parameter2);

            // assert
            Assert.IsTrue(actualResult is OkObjectResult);
            decimal acturalResultValue = (decimal)(actualResult as OkObjectResult).Value;
            Assert.AreEqual(expectedResultValue, acturalResultValue);
        }
    }
}
