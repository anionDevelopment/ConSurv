using ContinuousSurveillanceBackend.Core.Constants;
using GRYLibrary.Core.Misc;
using Microsoft.Extensions.DependencyInjection;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using DNWAPICUUtilities = ContinuousSurveillanceBackend.Core.Miscellaneous.Utilities;
using ContinuousSurveillanceBackend.Core.Configuration;
using ContinuousSurveillanceBackend.Core.Services;
using ContinuousSurveillanceBackend.Core.BackgroundServices;
using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.Misc.FilePath;
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
using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using OpenTelemetry.Metrics;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Services.TS;
using GRYLibrary.Core.APIServer.Services.Cred;
using GRYLibrary.Core.APIServer.Services.CredC;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.Services.Auth;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GRYLibrary.Core.Misc.FilePath;

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
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.SomeBackgroundServiceSettings = new CameraSchedulerServiceSettings()
                    {
                        Enabled = runServices,
                        LogConfiguration = GRYLogConfiguration.GetCommonConfiguration(AbstractFilePath.FromString($"./{nameof(CameraSchedulerService)}.log"), verbose),
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.CameraSchedulerServiceSettings = new CameraSchedulerServiceSettings()
                    {
                        VideoLength = TimeSpan.FromMinutes(10),
                        StorageFolder = Path.Combine(initializationInformation.ApplicationConstants.GetDataFolder(), "Recordings"),
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration = new AuthenticationConfiguration()
                    {
                        RoutesWhereUnauthenticatedAccessIsAllowed = new HashSet<string>() {
                            Regex.Escape("/API/Other/Resources/APISpecification/")+"*",
                        },
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthorizationConfiguration = new AuthorizationConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.CookieServiceConfiguration = new CookieServiceConfiguration()
                    {
                        CookieName = GeneralConstants.CodeUnitName,
                    };
                     initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware = new DRequestLoggingConfiguration()
                    {
                        NotLoggedRoutes = new HashSet<string>()
                        {
                            @$"^/favicon\.ico$",
                            @$"^/Web/.*$",
                        },
                        MaximalLengthofResponseBodies = 50,
                    };
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

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICredentialsProvider, CookieService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICookieServiceConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.CookieServiceConfiguration);

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService<User>, TransientAuthenticationService<User>>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService>(sp => sp.GetRequiredService<IAuthenticationService<User>>());

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITransientAuthenticationServicePersistence<User>, TransientAuthenticationServicePersistence<User>>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<User>>(sp => sp.GetRequiredService<ITransientAuthenticationServicePersistence<User>>());

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRoleBasedAuthorizationService, StaticRoleBasedUserAuthorizationService<User>>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IUserAuthorizationService>(sp => sp.GetRequiredService<IRoleBasedAuthorizationService>());
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthorizationService>(sp => sp.GetRequiredService<IUserAuthorizationService>());


                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITimeService, TimeService>();

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICameraSchedulerService, CameraSchedulerService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICameraSchedulerServiceSettings>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.SomeBackgroundServiceSettings);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICommonRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMaintenanceRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForLoggingMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware);

                    functionalInformation.WebApplicationBuilder.Services.AddHealthChecks().AddCheck<HealthCheck>(nameof(HealthCheck));

                    ServiceProvider serviceProvider = functionalInformation.WebApplicationBuilder.Services.BuildServiceProvider();

                    functionalInformation.WebApplicationBuilder.Services.AddOpenTelemetry().WithMetrics(builder =>
                    {
                        builder.AddMeter(CodeUnitSpecificConstants.AvailableCamerasRatioMeterName);
                        builder.AddPrometheusExporter();
                    });
                };
                apiServerConfiguration.ConfigureWebApplication = (functionalInformationForWebApplication) =>
                {
                    Initialize(functionalInformationForWebApplication.WebApplication.Services.GetService<IAuthenticationService>(), functionalInformationForWebApplication.WebApplication.Services.GetService<ITimeService>());
                    ICameraSchedulerService someBackgroundService = functionalInformationForWebApplication.WebApplication.Services.GetService<ICameraSchedulerService>();
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
        public static void Initialize(IAuthenticationService authenticationService, ITimeService timeService)
        {
            if (!authenticationService.UserExistsByName(CodeUnitSpecificConstants.UserNameAdmin))
            {
                authenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserGroupUser);
                Role userRole = authenticationService.GetRoleByName(CodeUnitSpecificConstants.UserGroupUser);

                authenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserGroupCameraManagers);
                Role cameraManagerRole = authenticationService.GetRoleByName(CodeUnitSpecificConstants.UserGroupCameraManagers);
                cameraManagerRole.InheritedRoles.Add(userRole);
                authenticationService.UpdateRole(cameraManagerRole);

                authenticationService.EnsureRoleExists(CodeUnitSpecificConstants.UserNameAdmin);
                Role adminRole = authenticationService.GetRoleByName(CodeUnitSpecificConstants.UserNameAdmin);
                adminRole.InheritedRoles.Add(cameraManagerRole);
                adminRole.InheritedRoles.Add(userRole);
                authenticationService.UpdateRole(adminRole);

                string adminUserName = "admin";
                string password = adminUserName;//initial password

                User adminUser = User.CreateNewUser("admin", authenticationService.Hash(password), out _, timeService);
                authenticationService.AddUser(adminUser);
                authenticationService.EnsureUserHasRole(adminUser.Id, adminRole.Id);
            }
        }
    }
}
