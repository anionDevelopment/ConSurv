using ConSurvBackend.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using System.Collections.Generic;
using System;
using GRYLibrary.Core.Misc;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer;

namespace ConSurvBackend.Tests.Testcases.Services
{
    [TestClass]
    public class BusinessLogicServiceTests
    {
        private void InitializeServices(bool registrationIsEnabled, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence)
        {
            ITimeService timeService = new TimeService();
            IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> persistedAPIServerConfiguration = new PersistedAPIServerConfiguration<CodeUnitSpecificConfiguration>
            {
                ApplicationSpecificConfiguration = new CodeUnitSpecificConfiguration
                {
                    RegistrationIsEnabled = registrationIsEnabled
                }
            };
            ApplicationConstants<CodeUnitSpecificConstants> constants = new ApplicationConstants<CodeUnitSpecificConstants>(GeneralConstants.CodeUnitName, GeneralConstants.CodeUnitVersion, Version3.Parse(GeneralConstants.CodeUnitVersion), TestRun.Instance, ConSurvBackend.Core.Misc.Utilities.GetEnvironmentTargetType(), new CodeUnitSpecificConstants());
            constants.BaseFolder = APIServer<CodeUnitSpecificConstants, PersistedAPIServerConfiguration<CodeUnitSpecificConfiguration>, CommandlineParameter>.GetDefaultBaseFolder(constants, true);
            IServerLog logger = new ServerLog(new GRYLogConfiguration(true), constants.GetLogFolder());
            IAuditLog auditLog = new AuditLog(new GRYLogConfiguration(true), constants.GetLogFolder());
            (TransientPersistence, ISet<IDisposable>) databasePersistence = ConSurvBackend.Tests.TestUtilities.Utilities.GetTransientPersistence();
            persistence = databasePersistence.Item1;
            persistence.Reset();
            IAuthenticationService<User> authenticationService = new PersistentAuthenticationService(timeService, persistence, logger);
            IGeneralResourceLoader generalResourceLoader = new ConSurvBackend.Core.Services.GeneralResourceLoader();
            IRandomnessProvider randomnessProvider = new RandomnessProvider(new System.Random());
            IRuntimeData runtimeData = new RuntimeData(generalResourceLoader, timeService);
            businessLogicService = new BusinessLogicService(persistence, logger, timeService, authenticationService, randomnessProvider, auditLog, persistedAPIServerConfiguration, runtimeData, constants);
            IExampleDataCreator exampleDataCreator = new ExampleDataCreator(persistence, authenticationService, timeService, logger, constants, businessLogicService, persistedAPIServerConfiguration);
            initializationService = new InitializationService(authenticationService, logger, businessLogicService, constants, exampleDataCreator, persistence);
        }

        [TestMethod(nameof(DatabaseInitializationTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void DatabaseInitializationTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence _);
            string adminUserName = CodeUnitSpecificConstants.UsernameAdmin;

            // act
            initializationService.Initialize(new CommandlineParameter());

            // assert
            Assert.IsTrue(businessLogicService.UserWithNameExists(adminUserName));
            // TODO add more assertions
        }

        [TestMethod(nameof(RegisterTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void RegisterTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string user = "someuser";
            string password = "somepassword";
            Assert.IsFalse(persistence.UserWithNameExists(user));

            // act
            string userId = businessLogicService.Register(user, password);

            // assert
            Assert.IsTrue(persistence.UserWithIdExists(userId));
            Assert.IsTrue(businessLogicService.UserWithNameExists(user));
            // TODO add more assertions
        }

        /// <remarks>
        /// A duplicate name must be refused by the business-logic, not only by the unique-constraint of the
        /// database: the constraint does not exist in the transient persistence and its violation would surface
        /// as an internal error instead of a usable one.
        /// </remarks>
        [TestMethod(nameof(RegisterWithAlreadyTakenUsernameIsRejectedTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void RegisterWithAlreadyTakenUsernameIsRejectedTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string user = "someuser";
            string userId = businessLogicService.Register(user, "somepassword");

            // act & assert
            Assert.ThrowsExactly<BadRequestException>(() => businessLogicService.Register(user, "anotherpassword"));

            // assert: the existing account is untouched and no second one was created
            Assert.IsTrue(persistence.UserWithIdExists(userId));
        }

        /// <remarks>
        /// The set of accepted color-schemes is a closed one. A value which is not part of it must be refused here,
        /// because the user-interface has no way to deal with an unknown value afterwards.
        /// </remarks>
        [TestMethod(nameof(SetThemeOfUserRejectsAnUnknownColorSchemeTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void SetThemeOfUserRejectsAnUnknownColorSchemeTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string userId = businessLogicService.Register("someuser", "somepassword");

            // act & assert
            Assert.ThrowsExactly<BadRequestException>(() => businessLogicService.SetThemeOfUser(userId, "neon-green"));

            // assert: the setting of the user is untouched
            Assert.AreEqual(CodeUnitSpecificConstants.ThemeSystem, businessLogicService.GetThemeOfUser(userId));
        }

        [TestMethod(nameof(SetThemeOfUserStoresEveryAcceptedColorSchemeTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void SetThemeOfUserStoresEveryAcceptedColorSchemeTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string userId = businessLogicService.Register("someuser", "somepassword");

            // act & assert
            foreach (string theme in CodeUnitSpecificConstants.Themes)
            {
                businessLogicService.SetThemeOfUser(userId, theme);
                Assert.AreEqual(theme, businessLogicService.GetThemeOfUser(userId), $"The color-scheme \"{theme}\" was not stored.");
            }
        }

        /// <remarks>
        /// A user who never chose a color-scheme must get the default instead of null, so that every caller can rely
        /// on getting a value which is part of the accepted set.
        /// </remarks>
        [TestMethod(nameof(GetThemeOfUserReturnsTheDefaultWhenNothingWasChosenTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void GetThemeOfUserReturnsTheDefaultWhenNothingWasChosenTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string userId = businessLogicService.Register("someuser", "somepassword");

            // act
            string theme = businessLogicService.GetThemeOfUser(userId);

            // assert
            Assert.AreEqual(CodeUnitSpecificConstants.ThemeSystem, theme);
        }

        /// <remarks>
        /// The camera-name ends up in the FFmpeg command-lines which are built for that camera and it is shown in the
        /// user-interface. A name which contains whitespace, a control-character (which includes carriage-return and
        /// line-feed), an invisible character or one of the shell-relevant characters has to be refused therefore.
        /// </remarks>
        [TestMethod(nameof(CreateCameraRejectsANameWhichWouldBreakTheCommandLineTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void CreateCameraRejectsANameWhichWouldBreakTheCommandLineTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string[] invalidNames = new string[] { "camera with space", "camera'name", "camera\"name", "camera|name", "camera&name", "camera*name", "camera/name", @"camera\name", "camera\rname", "camera\nname", "camera\tname", "camera\u0000name", "camera\u0007name", "camera\u007Fname", "camera\u200Bname", "camera\u00ADname" };

            // act & assert
            foreach (string invalidName in invalidNames)
            {
                Assert.ThrowsExactly<BadRequestException>(() => businessLogicService.CreateCamera(invalidName, "rtsp://example.local/stream"), $"The camera-name \"{invalidName}\" must not be accepted.");
            }
            Assert.ThrowsExactly<BadRequestException>(() => businessLogicService.CreateCamera(null, "rtsp://example.local/stream"));
        }

        /// <remarks>
        /// The camera-id is derived from the stream-URL alone. Two cameras which are created with the same URL - which
        /// is always the case for cameras created with the default-URL and adjusted afterwards - must therefore still
        /// get different ids, because the second one would otherwise replace the first one.
        /// </remarks>
        [TestMethod(nameof(TwoCamerasWithTheSameStreamUrlGetDifferentIdsTest))]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void TwoCamerasWithTheSameStreamUrlGetDifferentIdsTest()
        {
            // arrange
            this.InitializeServices(true, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence);
            initializationService.Initialize(new CommandlineParameter());
            string streamURL = "rtsp://example.local/stream";
            int amountOfCamerasBefore = businessLogicService.GetAllCameras().Count;

            // act
            string firstId = businessLogicService.CreateCamera("first-camera", streamURL);
            string secondId = businessLogicService.CreateCamera("second-camera", streamURL);

            // assert
            Assert.AreNotEqual(firstId, secondId);
            Assert.AreEqual(amountOfCamerasBefore + 2, businessLogicService.GetAllCameras().Count);
        }

        //TODO write testcases for the things which are not allowed to verify the user is really not able to do certain things
    }
}
