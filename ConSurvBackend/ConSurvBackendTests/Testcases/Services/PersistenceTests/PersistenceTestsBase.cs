using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Controller;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Auth.R;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConSurvBackend.Tests.Testcases.Services.PersistenceTests
{
    public abstract class PersistenceTestsBase
    {
        public PersistenceTestsBase()
        {
        }

        internal abstract PersistenceDisposable GetPersistence();

        public abstract void CreateCameraTest();
        public void CreateCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                Camera testCamera = new Camera("id", "name");
                Assert.IsFalse(persistenceD.Persistence.IsCamera(testCamera.Id));

                //act
                persistenceD.Persistence.CreateCamera(testCamera);

                //assert
                Assert.IsTrue(persistenceD.Persistence.IsCamera(testCamera.Id));
            }
        }

        public abstract void RemoveCameraTest();
        public void RemoveCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                Camera testCamera = new Camera("id", "name");
                persistence.CreateCamera(testCamera);
                Assert.IsTrue(persistence.IsCamera(testCamera.Id));

                //act
                persistence.RemoveCamera(testCamera.Id);

                //assert
                Assert.IsFalse(persistence.IsCamera(testCamera.Id));
            }
        }

        public abstract void UpdateCameraTest();
        public void UpdateCamera()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                Camera originalCamera = new Camera("id", "original-name");
                persistence.CreateCamera(originalCamera);
                Camera updatedCamera = new Camera("id", "updated-name");

                //act
                persistence.UpdateCamera(updatedCamera);

                //assert
                IDictionary<string, Camera> allCameras = persistence.GetAllCameras();
                Assert.IsTrue(allCameras.ContainsKey("id"));
                Assert.AreEqual("updated-name", allCameras["id"].Name);
            }
        }

        public abstract void GetAllCamerasTest();
        public void GetAllCameras()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                IDictionary<string, Camera> empty = persistence.GetAllCameras();
                Assert.AreEqual(0, empty.Count);
                Camera firstCamera = new Camera("id1", "first");
                Camera secondCamera = new Camera("id2", "second");
                persistence.CreateCamera(firstCamera);
                persistence.CreateCamera(secondCamera);

                //act
                IDictionary<string, Camera> allCameras = persistence.GetAllCameras();

                //assert
                Assert.AreEqual(2, allCameras.Count);
                Assert.IsTrue(allCameras.ContainsKey("id1"));
                Assert.IsTrue(allCameras.ContainsKey("id2"));
                Assert.AreEqual("first", allCameras["id1"].Name);
                Assert.AreEqual("second", allCameras["id2"].Name);
            }
        }

        public abstract void ResetTest();
        public void Reset()
        {
            // Note: post-Reset state differs between backends. TransientPersistence.Reset clears
            // the camera dictionary; DatabasePersistence.Reset drops the schema, so a subsequent
            // IsCamera/GetAllCameras call requires fresh migrations. The shared assertion is
            // therefore limited to "Reset must execute without throwing".
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                IPersistence persistence = persistenceD.Persistence;
                persistence.CreateCamera(new Camera("id1", "first"));
                persistence.CreateCamera(new Camera("id2", "second"));

                //act & assert: must not throw
                persistence.Reset();
            }
        }

        public abstract void UserCanNotDoAdministratorOnlyActionTest();
        public void UserCanNotDoAdministratorOnlyAction()
        {
            lock (ConSurvBackend.Tests.TestUtilities.Utilities.LockForTests)
            {
                //arrange
                using PersistenceDisposable persistenceD = this.GetPersistence();
                ServicesForTests.CreateServices(persistenceD.Persistence, new TimeService(), true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IAuthenticationService<User> authenticationService);
                initializationService.Initialize(new CommandlineParameter());
                string userName = $"user-{Guid.NewGuid()}";
                string password = "password";
                businessLogicService.Register(userName, password);
                //creating a user is an administrator-only action: the authorization-middleware only lets callers pass whose roles match this attribute.
                ISet<string> groupsAllowedToCreateUsers = typeof(UserController).GetMethod(nameof(UserController.CreateUser))!.GetCustomAttribute<AuthorizeAttribute>()!.Groups;
                IRoleBasedAuthorizationService authorizationService = new StaticRoleBasedUserAuthorizationService<User>();
                //the administrator is authorized, so a refusal for the user is caused by the missing role and not by a broken setup.
                Assert.IsTrue(IsAuthorized(authenticationService, authorizationService, CodeUnitSpecificConstants.UsernameAdmin, CodeUnitSpecificConstants.UsernameAdmin, groupsAllowedToCreateUsers));

                //act
                bool userIsAuthorized = IsAuthorized(authenticationService, authorizationService, userName, password, groupsAllowedToCreateUsers);

                //assert
                Assert.IsFalse(userIsAuthorized, "A user who is not an administrator must not be allowed to do an administrator-only action.");
            }
        }

        /// <summary>Does the same authorization-check as the authorization-middleware: the user is resolved by their access-token from the persistence and their roles are compared with the authorized groups.</summary>
        private static bool IsAuthorized(IAuthenticationService<User> authenticationService, IRoleBasedAuthorizationService authorizationService, string userName, string password, ISet<string> authorizedGroups)
        {
            AccessToken accessToken = authenticationService.Login(userName, password);
            User user = authenticationService.GetUserByAccessToken(accessToken.Value);
            return authorizationService.IsAuthorized(user.GetAllRoles().Select(role => role.Name).ToHashSet(), authorizedGroups);
        }
    }
}
