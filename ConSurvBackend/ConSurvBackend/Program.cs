using ConSurvBackend.Core.BackgroundServices;
using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Database;
using ConSurvBackend.Core.Misc;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.APIServer.MaintenanceRoutes;
using GRYLibrary.Core.APIServer.Mid.AuthS;
using GRYLibrary.Core.APIServer.Mid.AutS;
using GRYLibrary.Core.APIServer.Mid.Ex;
using GRYLibrary.Core.APIServer.Mid.M05DLog;
using GRYLibrary.Core.APIServer.MidT.Aut;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.MidT.Exception;
using GRYLibrary.Core.APIServer.MidT.RLog;
using GRYLibrary.Core.APIServer.Services.Auth.R;
using GRYLibrary.Core.APIServer.Services.CredH;
using GRYLibrary.Core.APIServer.Services.Database.DatabaseInterator;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using GRYLibrary.Core.Misc.FilePath;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core
{
    internal class Program
    {
        internal static int Main(string[] commandlineArguments)
        {
            bool runningUsually = false;
            return Tools.RunAPIServer<CommandlineParameter, CodeUnitSpecificConstants, CodeUnitSpecificConfiguration>(GeneralConstants.CodeUnitName, GeneralConstants.CodeUnitDescription, Version3.Parse(GeneralConstants.CodeUnitVersion), Misc.Utilities.GetEnvironmentTargetType(), GUtilities.GetExecutionMode(commandlineArguments), commandlineArguments, null, (apiServerConfiguration) =>
            {
                apiServerConfiguration.SetInitialzationInformationAction = (initializationInformation) =>
                {
                    runningUsually = initializationInformation.ApplicationConstants.ExecutionMode is RunProgram;
                    string domain = Tools.GetDefaultDomainValue(GeneralConstants.CodeUnitName);
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.SetDomainAndPublichUrlToDefault(domain);
                    initializationInformation.ApplicationConstants.AuthenticationMiddleware = typeof(AuthSMiddleware);
                    initializationInformation.ApplicationConstants.AuthorizationMiddleware = typeof(AutSRMiddleware);
                    initializationInformation.ApplicationConstants.ExceptionManagerMiddleware = typeof(DefaultExceptionHandlerMiddleware);
                    initializationInformation.ApplicationConstants.LoggingMiddleware = typeof(DRequestLoggingMiddleware);
                    initializationInformation.ApplicationConstants.CommonRoutesHostInformation = new HostCommonRoutes();
                    initializationInformation.ApplicationConstants.HostMaintenanceInformation = new HostMaintenanceRoutes();
                    initializationInformation.ApplicationConstants.UseWebSockets = true;
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForExceptionManagerMiddleware = new ExceptionManagerConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation = new CommonRoutesInformation()
                    {
                        ContactLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/Contact",
                        LicenseLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/License",
                        TermsOfServiceLink = $"https://information.{domain}/Products/{GeneralConstants.CodeUnitName}/TermsOfService"
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation = new MaintenanceRoutesInformation();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration = new DatabasePersistenceConfiguration()
                    {
                        DatabaseConnectionString = "Server=consurv_database;Port=5432;Database=ConSurvDatabase;UID=user;PWD=pa55w0rd;Search Path=public;",
                        DatabaseType = Debugger.IsAttached ? "Transient" : "PostgreSQL",
                    };
                    bool runServices = !runningUsually;
                    bool verbose = initializationInformation.ApplicationConstants.Environment is not Productive;

                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration = new AuthSConfiguration()
                    {
                        RoutesWhereUnauthenticatedAccessIsAllowed = new HashSet<string>() {
                                @$"^/API/Other/Resources/APISpecification/*",
                                @$"^/API/Other/Maintenance/HealthCheck$",
                        },
                    };
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Protocol = new HTTP();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthorizationConfiguration = new AutSRConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.HeaderServiceConfiguration = new HeaderServiceConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.TimeInUTC = false;
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.VideoLength = TimeSpan.FromMinutes(10);
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.TargetFolder = Path.Combine(initializationInformation.ApplicationConstants.GetDataFolder(), "Recordings");
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuditLogConfiguration = GRYLogConfiguration.GetCommonConfiguration(AbstractFilePath.FromString("./AuditLog.log"), true);
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware = new DRequestLoggingConfiguration()
                    {
                        NotLoggedRoutes = new HashSet<string>()
                        {
                                @$"^/favicon\.ico$",
                                @$"^/API/Other/Resources/APISpecification/*",
                        },
                        MaximalLengthOfResponseBodies = 50,
                    };
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment = true;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Domain = domain;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.DevelopmentCertificatePasswordHex = GeneralConstants.DevelopmentCertificatePasswordHex;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.DevelopmentCertificatePFXHex = GeneralConstants.DevelopmentCertificatePFXHex;
                };
                apiServerConfiguration.SetFunctionalInformationAction = (functionalInformation) => //technical initialization for every run
                {
                    functionalInformation.Logger.Log("Run initialization...");
                    IAuditLog auditLog = new AuditLog(functionalInformation.InitializationInformation.ApplicationConstants.ExecutionMode.Accept(new GetLoggerVisitor(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuditLogConfiguration, functionalInformation.InitializationInformation.ApplicationConstants.GetLogFolder(), "AuditLog")));
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuditLog>(auditLog);
                    IGeneralLogger logger = functionalInformation.Logger;
                    bool useDatabase = functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType != "Transient" && !runningUsually;
                    if (useDatabase)
                    {
                        logger.Log($"Run persistent using database \"{functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType}\".", LogLevel.Information);
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService<User>, PersistentAuthenticationService>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<User>>(sp => sp.GetRequiredService<IPersistence>());
                        IGenericDatabaseInteractor genericDatabaseInteractor;
                        if (functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType == "PostgreSQL")
                        {
                            genericDatabaseInteractor = new PostgreSQLDatabaseInteractor();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, DatabasePostgreSQLPersistence>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDatabaseManager, DatabaseManagerPostgreSQL>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISQLProvider, SQLProviderPostgreSQL>();
                            functionalInformation.WebApplicationBuilder.Services.AddDbContext<DatabaseContext>(options =>
                            {
                                string connectionString = functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseConnectionString;
                                /*
                                    options.UseNpgsql(connectionString, sqlOptions =>
                                {
                                    sqlOptions.CommandTimeout(120);
                                });
                                */
                            }, ServiceLifetime.Singleton);
                        }
                        else if (functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType == "MariaDB")
                        {
                            genericDatabaseInteractor = new MariaDBDatabaseInteractor();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, GenericPersistence>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDatabaseManager, DatabaseManagerMariaDB>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISQLProvider, SQLProviderMariaDB>();
                            functionalInformation.WebApplicationBuilder.Services.AddDbContext<DatabaseContext>(options =>
                            {
                                string connectionString = functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseConnectionString;
                                /*
                                 options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sqlOptions =>
                                 {
                                     sqlOptions.CommandTimeout(120);
                                 });
                                */
                            }, ServiceLifetime.Singleton);
                        }
                        else
                        {
                            throw new NotSupportedException("Database not supported. For a list of supported databases see the documentation.");
                        }
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IGenericDatabaseInteractor>(genericDatabaseInteractor);
                    }
                    else
                    {
                        logger.Log($"Run transient.", LogLevel.Information);
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, TransientPersistence>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService<User>, TransientAuthenticationService<User>>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITransientAuthenticationServicePersistence<User>, TransientAuthenticationServicePersistence<User>>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<User>>(sp => sp.GetRequiredService<ITransientAuthenticationServicePersistence<User>>());
                    }
                    bool useMockService = runningUsually;
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IStreamOrganizerService, StreamOrganizerService>();
                    if (functionalInformation.InitializationInformation.ApplicationConstants.Environment is Development && useMockService)
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRTSPManager, RTSPManagerMock>();
                    }
                    else
                    {
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRTSPManager, RTSPManager>();
                    }
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPreviewService, PreviewService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IGeneralResourceLoader, Misc.GeneralResourceLoader>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService>(sp => sp.GetRequiredService<IAuthenticationService<User>>());

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRoleBasedAuthorizationService, StaticRoleBasedUserAuthorizationService<User>>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IUserAuthorizationService>(sp => sp.GetRequiredService<IRoleBasedAuthorizationService>());
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthorizationService>(sp => sp.GetRequiredService<IUserAuthorizationService>());

                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICredentialsProvider, HeaderService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITimeService, TimeService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRandomnessProvider>(new RandomnessProvider(new Random(42)));
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IHealthCheck, HealthCheck>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISQLProvider, SQLProviderMariaDB>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMetricsService, MetricsService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IBusinessLogicService, BusinessLogicService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IHeaderServiceConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.HeaderServiceConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICommonRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.CommonRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMaintenanceRoutesInformation>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForLoggingMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDRequestLoggingConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthSConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForAuthenticationMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAutSRConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthorizationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthorizationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.ConfigurationForAuthorizationMiddleware);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IInitializationService<CommandlineParameter>, InitializationService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IExampleDataCreator, ExampleDataCreator>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IProcessManager, ProcessManager>();
                    functionalInformation.WebApplicationBuilder.Services.AddHealthChecks().AddCheck<HealthCheck>(nameof(HealthCheck));
                };
                apiServerConfiguration.ConfigureWebApplication = (functionalInformationForWebApplication) =>
                {
                    try
                    {
                        if (!runningUsually)
                        {

                            IGeneralLogger logger = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IGeneralLogger>());
                            bool showInitialData = false;//disabled because it would reveal sensitive information in the log
                            if (showInitialData)
                            {
                                logger.Log($"{nameof(functionalInformationForWebApplication.InitializationInformation.CommandlineParameter.InitialAdminPassword)}: \"{functionalInformationForWebApplication.InitializationInformation.CommandlineParameter.InitialAdminPassword}\"", LogLevel.Information);
                                logger.Log($"{nameof(functionalInformationForWebApplication.InitializationInformation.CommandlineParameter.InitialCameraAddresses)}: \"{string.Join(", ", functionalInformationForWebApplication.InitializationInformation.CommandlineParameter.InitialCameraAddresses)}\"", LogLevel.Information);
                            }
                            logger.Log("Configure webapplication...", LogLevel.Information);
                            IInitializationService<CommandlineParameter> initializationService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IInitializationService<CommandlineParameter>>());
                            initializationService.Initialize(apiServerConfiguration.CommandlineParameter);

                            IMetricsService metricsService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IMetricsService>());
                            IPreviewService previewService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IPreviewService>());
                            functionalInformationForWebApplication.PreRun = () =>
                                {
                                    metricsService.StartAsync();
                                    previewService.StartAsync();
                                    //TODO start motion-detection-service
                                };
                            functionalInformationForWebApplication.PostRun = () =>
                                {
                                    metricsService.Stop().Wait();
                                    previewService.Stop().Wait();
                                    //TODO stop motion-detection-service
                                };
                        }
                    }
                    catch
                    {
                        throw;
                    }
                };
            });
        }
    }
}
