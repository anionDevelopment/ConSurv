using ConSurvBackend.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using System.Collections.Generic;
using System;
using GRYLibrary.Core.Misc;
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
        [TestProperty(nameof(TestKind), nameof(TestKind.IntegrationTest))]
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
        [TestProperty(nameof(TestKind), nameof(TestKind.IntegrationTest))]
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

        //TODO write testcases for the things which are not allowed to verify the user is really not able to do certain things
    }
}
