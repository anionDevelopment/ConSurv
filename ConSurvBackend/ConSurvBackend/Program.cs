using ConSurvBackend.Core.Constants;
using GRYLibrary.Core.Misc;
using Microsoft.Extensions.DependencyInjection;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Services;
using ConSurvBackend.Core.BackgroundServices;
using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.Utilities;
using ConSurvBackend.Core.Miscellaneous;
using GRYLibrary.Core.APIServer.MaintenanceRoutes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.EntityFrameworkCore;
using GRYLibrary.Core.APIServer.Mid.AuthS;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using System;
using System.IO;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.MidT.Auth;
using System.Collections.Generic;
using GRYLibrary.Core.APIServer.Services.Auth.R;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Mid.M05DLog;
using ConSurvBackend.Core.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Mid.Ex;
using GRYLibrary.Core.APIServer.MidT.Exception;
using GRYLibrary.Core.APIServer.Services.CredH;
using GRYLibrary.Core.APIServer.Mid.AutS;
using GRYLibrary.Core.APIServer.MidT.RLog;
using GRYLibrary.Core.APIServer.MidT.Aut;
using ConSurvBackend.Core.Controller;
using GRYLibrary.Core.APIServer.Services.OtherServices;

namespace ConSurvBackend.Core
{
    internal class Program
    {
        internal static int Main(string[] commandlineArguments)
        {
            return Tools.RunAPIServer<CommandlineParameter, CodeUnitSpecificConstants, CodeUnitSpecificConfiguration>(GeneralConstants.CodeUnitName, GeneralConstants.CodeUnitDescription, Version3.Parse(GeneralConstants.CodeUnitVersion), Miscellaneous.Utilities.GetEnvironmentTargetType(), GUtilities.GetExecutionMode(commandlineArguments), commandlineArguments, (apiServerConfiguration) =>
            {
                apiServerConfiguration.SetInitialzationInformationAction = (initializationInformation) => //HINT initialization for first run (used when configuration-file not exists)
                {
                    //TODO configure regarding to mode-parameter
                    string domain = Tools.GetDefaultDomainValue(GeneralConstants.CodeUnitName);
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.SetDomainAndPublichUrlToDefault(domain);
                    initializationInformation.ApplicationConstants.KnownTypes.Add(typeof(CameraController));//TODO check if this line can be removed
                    initializationInformation.ApplicationConstants.KnownTypes.Add(typeof(UserController));//TODO check if this line can be removed
                    initializationInformation.ApplicationConstants.AuthenticationMiddleware = typeof(AuthSMiddleware);
                    initializationInformation.ApplicationConstants.AuthorizationMiddleware = typeof(AutSRMiddleware);
                    initializationInformation.ApplicationConstants.ExceptionManagerMiddleware = typeof(DefaultExceptionHandlerMiddleware);
                    initializationInformation.ApplicationConstants.LoggingMiddleware = typeof(DRequestLoggingMiddleware);
                    initializationInformation.ApplicationConstants.CommonRoutesHostInformation = new HostCommonRoutes();
                    initializationInformation.ApplicationConstants.HostMaintenanceInformation = new HostMaintenanceRoutes();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForExceptionManagerMiddleware = new ExceptionManagerConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation = new CommonRoutesInformation()
                    {
                        ContactLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/Contact",
                        LicenseLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/License",
                        TermsOfServiceLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/TermsOfService"
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation = new MaintenanceRoutesInformation();
                    bool runServices = initializationInformation.ApplicationConstants.ExecutionMode is RunProgram;
                    bool verbose = initializationInformation.ApplicationConstants.Environment is not Productive;

                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration = new AuthSConfiguration()
                    {
                        RoutesWhereUnauthenticatedAccessIsAllowed = new HashSet<string>() {
                                @$"^/API/Other/Resources/APISpecification/*",
                        },
                    };
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Protocol = new HTTP();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthorizationConfiguration = new AutSRConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.HeaderServiceConfiguration = new HeaderServiceConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.TimeInUTC = false;
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.VideoLength = TimeSpan.FromMinutes(10);
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.TargetFolder = Path.Combine(initializationInformation.ApplicationConstants.GetDataFolder(), "Recordings");
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware = new DRequestLoggingConfiguration()
                    {
                        NotLoggedRoutes = new HashSet<string>()
                        {
                            @$"^/favicon\.ico$",
                            @$"^/API/Other/Resources/APISpecification/*",
                        },
                        MaximalLengthofResponseBodies = 50,
                    };
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment = true;
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
                            Tools.ConnectToDatabaseWrapper(() => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sqlOptions => { sqlOptions.CommandTimeout(120); }), GeneralLogger.NoLog(), GUtilities.AdaptMariaDBSQLConnectionString(connectionString, true));
                        }, ServiceLifetime.Singleton);
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, DatabasePersistence>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<Model.User>>(sp => sp.GetRequiredService<IPersistence>());
                    }
                    else
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, TransientPersistence>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITransientAuthenticationServicePersistence<User>, TransientAuthenticationServicePersistence<User>>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<User>>(sp => sp.GetRequiredService<ITransientAuthenticationServicePersistence<User>>());
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService<User>, TransientAuthenticationService<User>>();
                    }
                    if (functionalInformation.InitializationInformation.ApplicationConstants.Environment is Development && false)//TODO remove "&& false"
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRTSPManager, RTSPManagerMock>();
                    }
                    else
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRTSPManager, RTSPManager>();
                    }
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService>(sp => sp.GetRequiredService<IAuthenticationService<User>>());

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRoleBasedAuthorizationService, StaticRoleBasedUserAuthorizationService<User>>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IUserAuthorizationService>(sp => sp.GetRequiredService<IRoleBasedAuthorizationService>());
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthorizationService>(sp => sp.GetRequiredService<IUserAuthorizationService>());

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICredentialsProvider, HeaderService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITimeService, TimeService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRandomnessProvider>(new RandomnessProvider(new Random(42)));
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IHealthCheck, HealthCheck>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISQLProvider, SQLProvider>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMetricsService, MetricsService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICameraService, CameraService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IHeaderServiceConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.HeaderServiceConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICommonRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMaintenanceRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForLoggingMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthSConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForAuthenticationMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAutSRConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthorizationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthorizationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForAuthorizationMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IInitializationService<ConSurvBackend.Core.Configuration.CommandlineParameter>, InitializationService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IExampleDataCreator, ExampleDataCreator>();
                    functionalInformation.WebApplicationBuilder.Services.AddHealthChecks().AddCheck<HealthCheck>(nameof(HealthCheck));
                };
                apiServerConfiguration.ConfigureWebApplication = (functionalInformationForWebApplication) =>
                {
                    IInitializationService<ConSurvBackend.Core.Configuration.CommandlineParameter>? initializationService = functionalInformationForWebApplication.WebApplication.Services.GetService<IInitializationService<ConSurvBackend.Core.Configuration.CommandlineParameter>>();
                    initializationService.Initialize(apiServerConfiguration.CommandlineParameter);

                    IMetricsService? metricsService = functionalInformationForWebApplication.WebApplication.Services.GetService<IMetricsService>();
                    functionalInformationForWebApplication.PreRun = () =>
                    {
                        metricsService.StartAsync();
                    };
                    functionalInformationForWebApplication.PostRun = () =>
                    {
                        metricsService.Stop().Wait();
                    };
                };
            });
        }
    }
}
