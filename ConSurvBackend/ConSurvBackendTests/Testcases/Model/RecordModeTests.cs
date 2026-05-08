using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.Testcases.Model
{
    [TestClass]
    public class RecordModeTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToNumber_NoRecording_Returns0()
        {
            // arrange & act
            byte result = RecordMode.ToNumber(typeof(NoRecording));

            // assert
            Assert.AreEqual((byte)0, result);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToNumber_RecordAlways_Returns1()
        {
            // arrange & act
            byte result = RecordMode.ToNumber(typeof(RecordAlways));

            // assert
            Assert.AreEqual((byte)1, result);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToNumber_RecordOnMovements_Returns2()
        {
            // arrange & act
            byte result = RecordMode.ToNumber(typeof(RecordOnMovements));

            // assert
            Assert.AreEqual((byte)2, result);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToNumber_UnknownType_ThrowsKeyNotFoundException()
        {
            // arrange
            bool threwExpectedException = false;

            // act
            try
            {
                RecordMode.ToNumber(typeof(string));
            }
            catch (KeyNotFoundException)
            {
                threwExpectedException = true;
            }

            // assert
            Assert.IsTrue(threwExpectedException);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void FromNumber_0_ReturnsNoRecordingType()
        {
            // arrange & act
            Type result = RecordMode.FromNumber(0);

            // assert
            Assert.AreEqual(typeof(NoRecording), result);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void FromNumber_1_ReturnsRecordAlwaysType()
        {
            // arrange & act
            Type result = RecordMode.FromNumber(1);

            // assert
            Assert.AreEqual(typeof(RecordAlways), result);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void FromNumber_2_ReturnsRecordOnMovementsType()
        {
            // arrange & act
            Type result = RecordMode.FromNumber(2);

            // assert
            Assert.AreEqual(typeof(RecordOnMovements), result);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void FromNumberToInstance_0_ReturnsNoRecordingInstance()
        {
            // arrange & act
            RecordMode result = RecordMode.FromNumberToInstance(0);

            // assert
            Assert.IsInstanceOfType(result, typeof(NoRecording));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void FromNumberToInstance_1_ReturnsRecordAlwaysInstance()
        {
            // arrange & act
            RecordMode result = RecordMode.FromNumberToInstance(1);

            // assert
            Assert.IsInstanceOfType(result, typeof(RecordAlways));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void FromNumberToInstance_2_ReturnsRecordOnMovementsInstance()
        {
            // arrange & act
            RecordMode result = RecordMode.FromNumberToInstance(2);

            // assert
            Assert.IsInstanceOfType(result, typeof(RecordOnMovements));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Equals_SameType_ReturnsTrue()
        {
            // arrange
            RecordMode a = new RecordAlways();
            RecordMode b = new RecordAlways();

            // act & assert
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // arrange
            RecordMode a = new RecordAlways();
            RecordMode b = new NoRecording();

            // act & assert
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void Equals_Null_ReturnsFalse()
        {
            // arrange
            RecordMode a = new RecordAlways();

            // act & assert
            Assert.IsFalse(a.Equals(null));
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToDTO_NoRecording_ReturnsCorrectDTOTypeName()
        {
            // arrange
            RecordMode recordMode = new NoRecording();

            // act
            RecordModeDTO result = recordMode.ToDTO();

            // assert
            Assert.AreEqual(nameof(NoRecording), result.RecordMode);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToDTO_RecordAlways_ReturnsCorrectDTOTypeName()
        {
            // arrange
            RecordMode recordMode = new RecordAlways();

            // act
            RecordModeDTO result = recordMode.ToDTO();

            // assert
            Assert.AreEqual(nameof(RecordAlways), result.RecordMode);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToDTO_RecordOnMovements_ReturnsCorrectDTOTypeName()
        {
            // arrange
            RecordMode recordMode = new RecordOnMovements();

            // act
            RecordModeDTO result = recordMode.ToDTO();

            // assert
            Assert.AreEqual(nameof(RecordOnMovements), result.RecordMode);
        }
    }
}
