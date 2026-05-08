using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConSurvBackend.Tests.Testcases.Model
{
    [TestClass]
    public class RecordModeDTOTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToRecordMode_NoRecording_ReturnsNoRecordingInstance()
        {
            // arrange
            RecordModeDTO dto = new RecordModeDTO { RecordMode = nameof(NoRecording) };

            // act
            RecordMode result = dto.ToRecordMode();

            // assert
            Assert.IsInstanceOfType(result, typeof(NoRecording));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToRecordMode_RecordAlways_ReturnsRecordAlwaysInstance()
        {
            // arrange
            RecordModeDTO dto = new RecordModeDTO { RecordMode = nameof(RecordAlways) };

            // act
            RecordMode result = dto.ToRecordMode();

            // assert
            Assert.IsInstanceOfType(result, typeof(RecordAlways));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToRecordMode_RecordOnMovements_ReturnsRecordOnMovementsInstance()
        {
            // arrange
            RecordModeDTO dto = new RecordModeDTO { RecordMode = nameof(RecordOnMovements) };

            // act
            RecordMode result = dto.ToRecordMode();

            // assert
            Assert.IsInstanceOfType(result, typeof(RecordOnMovements));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToRecordMode_UnknownMode_ThrowsNotSupportedException()
        {
            // arrange
            RecordModeDTO dto = new RecordModeDTO { RecordMode = "NonExistentMode" };
            bool threwExpectedException = false;

            // act
            try
            {
                dto.ToRecordMode();
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
        public void ToRecordMode_RoundTrip_NoRecording_PreservesType()
        {
            // arrange
            RecordMode original = new NoRecording();
            RecordModeDTO dto = original.ToDTO();

            // act
            RecordMode restored = dto.ToRecordMode();

            // assert
            Assert.IsTrue(original.Equals(restored));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToRecordMode_RoundTrip_RecordAlways_PreservesType()
        {
            // arrange
            RecordMode original = new RecordAlways();
            RecordModeDTO dto = original.ToDTO();

            // act
            RecordMode restored = dto.ToRecordMode();

            // assert
            Assert.IsTrue(original.Equals(restored));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToRecordMode_RoundTrip_RecordOnMovements_PreservesType()
        {
            // arrange
            RecordMode original = new RecordOnMovements();
            RecordModeDTO dto = original.ToDTO();

            // act
            RecordMode restored = dto.ToRecordMode();

            // assert
            Assert.IsTrue(original.Equals(restored));
        }
    }
}
