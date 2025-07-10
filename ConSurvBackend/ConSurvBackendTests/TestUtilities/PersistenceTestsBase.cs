using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Misc.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Services;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.TestUtilities
{
    public abstract class PersistenceTestsBase
    {
        public PersistenceTestsBase()
        {
        }

        public abstract IPersistence GetPersistence();


        public abstract void AddCameraTest();
        public void AddCamera()
        {
            //arrange
            using IPersistence persistence = this.GetPersistence();
            Camera testCamera = new Camera("ABCDEF", "Camera1");
            Assert.IsFalse(persistence.IsCamera(testCamera.Id));

            //act
            persistence.CreateCamera(testCamera);

            //assert
            Assert.IsTrue(persistence.IsCamera(testCamera.Id));
        }
    }
}
