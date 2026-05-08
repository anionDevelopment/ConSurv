using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.MoveDirections;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.ZoomDirections;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConSurvBackend.Tests.Testcases.Model
{
    [TestClass]
    public class ONVIFCommandDTOTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Move_Up_ReturnsMoveWithMoveUpDirection()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Move), Direction = nameof(MoveUp) };

            // act
            ONVIFCommand result = dto.ToONVIFCommand();

            // assert
            Move move = result as Move;
            Assert.IsNotNull(move);
            Assert.IsInstanceOfType(move.MoveDirection, typeof(MoveUp));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Move_Down_ReturnsMoveWithMoveDownDirection()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Move), Direction = nameof(MoveDown) };

            // act
            ONVIFCommand result = dto.ToONVIFCommand();

            // assert
            Move move = result as Move;
            Assert.IsNotNull(move);
            Assert.IsInstanceOfType(move.MoveDirection, typeof(MoveDown));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Move_Left_ReturnsMoveWithMoveLeftDirection()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Move), Direction = nameof(MoveLeft) };

            // act
            ONVIFCommand result = dto.ToONVIFCommand();

            // assert
            Move move = result as Move;
            Assert.IsNotNull(move);
            Assert.IsInstanceOfType(move.MoveDirection, typeof(MoveLeft));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Move_Right_ReturnsMoveWithMoveRightDirection()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Move), Direction = nameof(MoveRight) };

            // act
            ONVIFCommand result = dto.ToONVIFCommand();

            // assert
            Move move = result as Move;
            Assert.IsNotNull(move);
            Assert.IsInstanceOfType(move.MoveDirection, typeof(MoveRight));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Zoom_In_ReturnsZoomWithZoomInDirection()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Zoom), Direction = nameof(ZoomIn) };

            // act
            ONVIFCommand result = dto.ToONVIFCommand();

            // assert
            Zoom zoom = result as Zoom;
            Assert.IsNotNull(zoom);
            Assert.IsInstanceOfType(zoom.ZoomDirection, typeof(ZoomIn));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Zoom_Out_ReturnsZoomWithZoomOutDirection()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Zoom), Direction = nameof(ZoomOut) };

            // act
            ONVIFCommand result = dto.ToONVIFCommand();

            // assert
            Zoom zoom = result as Zoom;
            Assert.IsNotNull(zoom);
            Assert.IsInstanceOfType(zoom.ZoomDirection, typeof(ZoomOut));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_UnknownCommandType_ThrowsNotSupportedException()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = "UnknownCommand", Direction = nameof(MoveUp) };
            bool threwExpectedException = false;

            // act
            try
            {
                dto.ToONVIFCommand();
            }
            catch (NotSupportedException)
            {
                threwExpectedException = true;
            }

            // assert
            Assert.IsTrue(threwExpectedException);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Move_UnknownDirection_ThrowsNotSupportedException()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Move), Direction = "UnknownDirection" };
            bool threwExpectedException = false;

            // act
            try
            {
                dto.ToONVIFCommand();
            }
            catch (NotSupportedException)
            {
                threwExpectedException = true;
            }

            // assert
            Assert.IsTrue(threwExpectedException);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToONVIFCommand_Zoom_UnknownDirection_ThrowsNotSupportedException()
        {
            // arrange
            ONVIFCommandDTO dto = new ONVIFCommandDTO { CommandType = nameof(Zoom), Direction = "UnknownDirection" };
            bool threwExpectedException = false;

            // act
            try
            {
                dto.ToONVIFCommand();
            }
            catch (NotSupportedException)
            {
                threwExpectedException = true;
            }

            // assert
            Assert.IsTrue(threwExpectedException);
        }
    }
}
