using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonDBTypes;

namespace ConSurvBackend.Tests.Testcases.Services
{
    [TestClass]
    public class BusinessLogicServiceTests
    {
        private void InitializeServices(bool registrationIsEnabled, out IBusinessLogicService businessLogicService, out IInitializationService<CommandlineParameter> initializationService, out IPersistence persistence)
        {
            ITimeService timeService = new TimeService();
            IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> persistedAPIServerConfiguration = new PersistedAPIServerConfiguration<CodeUnitSpecificConfiguration>();
            persistedAPIServerConfiguration.ApplicationSpecificConfiguration = new CodeUnitSpecificConfiguration();
            persistedAPIServerConfiguration.ApplicationSpecificConfiguration.RegistrationIsEnabled = registrationIsEnabled;
            IGRYLog logger = GeneralLogger.CreateUsingConsole();
            IAuditLog auditLog = new AuditLog(GeneralLogger.CreateUsingConsole());
            ISQLProvider sqlProvider = new SQLProviderPostgreSQL(logger);
            IPersistence databasePersistence = ConSurvBackend.Tests.TestUtilities.Utilities.GetTransientPersistence();
            persistence = databasePersistence;
            persistence.Reset();
            IApplicationConstants<CodeUnitSpecificConstants> constants = new ApplicationConstants<CodeUnitSpecificConstants>(GeneralConstants.CodeUnitName, GeneralConstants.CodeUnitVersion, Version3.Parse(GeneralConstants.CodeUnitVersion), RunProgram.Instance, QualityCheck.Instance, new CodeUnitSpecificConstants());
            IAuthenticationService<User> authenticationService = new PersistentAuthenticationService(timeService, databasePersistence);
            IGeneralResourceLoader generalResourceLoader = new ConSurvBackend.Core.Services.GeneralResourceLoader();
            Mock<IStreamOrganizerService> streamOrganizerServiceMock = new Mock<IStreamOrganizerService>(MockBehavior.Strict);
            streamOrganizerServiceMock.Setup(m => m.InitializeCameraOrganization());
            Mock<IRTSPManager> rtspManagerMock = new Mock<IRTSPManager>(MockBehavior.Strict);
            IRandomnessProvider randomnessProvider = new RandomnessProvider(new System.Random());
            businessLogicService = new BusinessLogicService(databasePersistence, logger, rtspManagerMock.Object, timeService, authenticationService, randomnessProvider, auditLog, streamOrganizerServiceMock.Object);
            IExampleDataCreator exampleDataCreator = new ExampleDataCreator(databasePersistence, authenticationService, timeService, logger, constants, businessLogicService, persistedAPIServerConfiguration);
            initializationService = new InitializationService(authenticationService, logger, businessLogicService, constants, exampleDataCreator, rtspManagerMock.Object, streamOrganizerServiceMock.Object);
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
