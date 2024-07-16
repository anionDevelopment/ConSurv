using ContinuousSurveillanceBackend.Core.Constants;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.Extensions.DependencyInjection;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using DNWAPICUUtilities = ContinuousSurveillanceBackend.Core.Miscellaneous.Utilities;
using ContinuousSurveillanceBackend.Core.Configuration;
using ContinuousSurveillanceBackend.Core.Services;
using ContinuousSurveillanceBackend.Core.BackgroundServices;
using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.Miscellaneous.FilePath;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GRYLogger;
using ContinuousSurveillanceBackend.Core.Miscellaneous;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using GRYLibrary.Core.APIServer.MidT.RLog;
using GRYLibrary.Core.APIServer.Mid.DLog;
using GRYLibrary.Core.APIServer.MaintenanceRoutes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.EntityFrameworkCore;
using ContinuousSurveillanceBackend.Core.Database.Contexts;
using GRYLibrary.Core.APIServer.Mid.AuthS;
using GRYLibrary.Core.APIServer.Mid.Auth;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.CommonDBTypes;

namespace ContinuousSurveillanceBackend.Core
{
    internal class Program
    {
        internal static int Main(string[] commandlineArguments)
        {
            return Tools.RunAPIServer<CodeUnitSpecificCommandlineParameter, CodeUnitSpecificConstants, CodeUnitSpecificConfiguration>(GeneralConstants.CodeUnitName, GeneralConstants.CodeUnitDescription, Version3.Parse(GeneralConstants.CodeUnitVersion), DNWAPICUUtilities.GetEnvironmentTargetType(), GUtilities.GetExecutionMode(commandlineArguments), commandlineArguments, (apiServerConfiguration) =>
            {
                apiServerConfiguration.SetInitialzationInformationAction = (initializationInformation) => //HINT initialization for first run (used when configuration-file not exists)
                {
                    string domain = Tools.GetDefaultDomainValue(GeneralConstants.CodeUnitName);
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.SetDomainAndPublichUrlToDefault(domain);
                    initializationInformation.ApplicationConstants.CommonRoutesHostInformation = new HostCommonRoutes();
                    initializationInformation.ApplicationConstants.HostMaintenanceInformation = new HostMaintenanceRoutes();
                    initializationInformation.ApplicationConstants.AuthenticationMiddleware = typeof(AuthSMiddleware);
                    initializationInformation.ApplicationConstants.AuthorizationMiddleware = typeof(AutSRMiddleware);
                    initializationInformation.ApplicationConstants.LoggingMiddleware = typeof(DRequestLoggingMiddleware);
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation = new CommonRoutesInformation()
                    {
                        ContactLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/Contact",
                        LicenseLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/License",
                        TermsOfServiceLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/TermsOfService"
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation = new MaintenanceRoutesInformation();
                    bool runServices = initializationInformation.ApplicationConstants.ExecutionMode is RunProgram;
                    bool verbose = initializationInformation.ApplicationConstants.Environment is not Productive;
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.SomeBackgroundServiceSettings = new SomeBackgroundServiceSettings()
                    {
                        Enabled = runServices,
                        LogConfiguration = GRYLogConfiguration.GetCommonConfiguration(AbstractFilePath.FromString($"./{nameof(SomeBackgroundService)}.log"), verbose),
                        SomePropertyOfTypeBool = true,
                        SomePropertyOfTypeInt = 42,
                        SomePropertyOfTypeString = "some string value",
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware = new DRequestLoggingConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment = true;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Protocol = initializationInformation.ApplicationConstants.ExecutionMode.Accept(new GetProcolVisitor(domain));
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Domain = domain;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.DevelopmentCertificatePasswordHex = GeneralConstants.DevelopmentCertificatePasswordHex;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.DevelopmentCertificatePFXHex = GeneralConstants.DevelopmentCertificatePFXHex;
                };
                apiServerConfiguration.SetFunctionalInformationAction = (functionalInformation) => //initialization for every run
                {
                    IGeneralLogger logger = functionalInformation.Logger;
                    bool runPersistent = functionalInformation.InitializationInformation.ApplicationConstants.Environment is not Development && functionalInformation.InitializationInformation.ApplicationConstants.ExecutionMode is RunProgram;
                    if (runPersistent)
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddDbContext<DatabaseContext>(options =>
                        {
                            string connectionString = functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabaseConnectionString;
                            Tools.ConnectToDatabase(() => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)), logger, GUtilities.AdaptMariaDBSQLConnectionString(connectionString, true));
                        }, ServiceLifetime.Transient);
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, DatabasePersistence>();
                    }
                    else
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, TransientPersistence>();
                    }
                    functionalInformation.WebApplicationBuilder.Services.AddHealthChecks().AddCheck<HealthCheck>(nameof(HealthCheck));
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISomeBackgroundService, SomeBackgroundService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISomeBackgroundServiceSettings>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.SomeBackgroundServiceSettings);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICommonRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMaintenanceRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForLoggingMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware);
                    ServiceProvider serviceProvider = functionalInformation.WebApplicationBuilder.Services.BuildServiceProvider();

                    functionalInformation.WebApplicationBuilder.Services.AddOpenTelemetry().WithMetrics(builder =>
                    {
                        //TODO add metrics
                    });
                };
                apiServerConfiguration.ConfigureWebApplication = (functionalInformationForWebApplication) =>
                {
                    Initialize(functionalInformationForWebApplication.WebApplication.Services.GetService<IAuthenticationService>());
                    ISomeBackgroundService someBackgroundService = functionalInformationForWebApplication.WebApplication.Services.GetService<ISomeBackgroundService>();
                    functionalInformationForWebApplication.PreRun = () =>
                    {
                        someBackgroundService.StartAsync();
                    };
                    functionalInformationForWebApplication.PostRun = () =>
                    {
                        someBackgroundService.Stop().Wait();
                    };
                    functionalInformationForWebApplication.WebApplication.MapHealthChecks(GRYLibrary.Core.APIServer.Utilities.Constants.UsualHealthCheckEndpoint);
                    functionalInformationForWebApplication.WebApplication.UseOpenTelemetryPrometheusScrapingEndpoint(GRYLibrary.Core.APIServer.Utilities.Constants.UsualMetricsEndpoint);
                };
            });
        }
        public static void Initialize(IAuthenticationService authenticationService)
        {
            authenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserNameAdmin);
            if (!authenticationService.UserExistsByName(CodeUnitSpecificConstants.UserNameAdmin))
            {
                User adminUser = new User();
                adminUser.Name = "admin";
                string password = adminUser.Name;
                adminUser.PasswordHash = authenticationService.Hash(password);
                adminUser.UserIsActivated = true;
                authenticationService.AddUser(adminUser);
                Role adminRole = authenticationService.GetRoleByName(CodeUnitSpecificConstants.UserNameAdmin);
                authenticationService.EnsureUserHasRole(adminUser.Id, adminRole.Id);
            }
        }
    }
}
