using ConSurvBackend.Core.BackgroundServices;
using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Constants;
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
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.OtherServices;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using GRYLibrary.Core.Misc.FilePath;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core
{
    internal class Program
    {
        internal bool ListenOnEveryIP { get; set; } = false;
        internal bool RunAsync { get; set; } = false;
        internal bool IsRunning { get; set; } = false;
        internal IBusinessLogicService? _BusinessLogicService;
        internal IInitializationService<CommandlineParameter>? _InitializationService;
        internal IGRYLog _Log;

        internal IHostApplicationLifetime? _HostApplicationLifetime;
        internal APIServerConfiguration<CodeUnitSpecificConstants, CodeUnitSpecificConfiguration, CommandlineParameter> _Constants;

        internal Action<FunctionalInformation<CodeUnitSpecificConstants, CodeUnitSpecificConfiguration, CommandlineParameter>> SetupMocks { get; set; }
        public Program()
        {
            this._Log = GRYLog.Create();
        }
        internal static int Main(string[] commandlineArguments)
        {
            return new Program().MainImplementation(commandlineArguments);
        }
        internal int MainImplementation(string[] commandlineArguments)
        {
            bool runningUsually = false;
            this.IsRunning = true;
            var result = Tools.RunAPIServer<CommandlineParameter, CodeUnitSpecificConstants, CodeUnitSpecificConfiguration>(GeneralConstants.CodeUnitName, GeneralConstants.CodeUnitDescription, Version3.Parse(GeneralConstants.CodeUnitVersion), Misc.Utilities.GetEnvironmentTargetType(), GUtilities.GetExecutionMode(commandlineArguments), commandlineArguments, null, (apiServerConfiguration) =>
            {
                apiServerConfiguration.SetInitialzationInformationAction = (initializationInformation) =>
                {
                    runningUsually = initializationInformation.ApplicationConstants.ExecutionMode is RunProgram;
                    var t1 = initializationInformation.ApplicationConstants.GetDataFolder();
                    var t2 = initializationInformation.ApplicationConstants.GetConfigurationFolder();
                    var t3 = initializationInformation.ApplicationConstants.GetLogFolder();

                    string domain = string.IsNullOrWhiteSpace(initializationInformation.CommandlineParameter.InitialDomain) ? Tools.GetDefaultDomainValue(GeneralConstants.CodeUnitName) : initializationInformation.CommandlineParameter.InitialDomain;
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
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.MaintenanceRoutesInformation = new MaintenanceRoutesInformation()
                    {
                        EnableEndpointAvailabilityCheck = initializationInformation.CommandlineParameter.InitialEnableEndpointAvailabilityCheckValue,
                        EnableEndpointInitializationState = initializationInformation.CommandlineParameter.InitialEnableEndpointInitializationStateValue,
                        EnableEndpointCurrentVersion = initializationInformation.CommandlineParameter.InitialEnableEndpointCurrentVersionValue,
                        EnableEndpointShowAllEndpoints = initializationInformation.CommandlineParameter.InitialEnableEndpointShowAllEndpointsValue,
                        EnableEndpointHealthCheck = initializationInformation.CommandlineParameter.InitialEnableEndpointHealthCheckValue,
                        EnableEndpointMetrics = initializationInformation.CommandlineParameter.InitialEnableEndpointMetricsValue,
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration = new DatabasePersistenceConfiguration()
                    {
                        DatabaseConnectionString = initializationInformation.CommandlineParameter.InitialDatabaseConnectionString ?? "insert your connection-string here",
                        DatabaseType = initializationInformation.CommandlineParameter.InitialDatabaseType ?? "Transient",
                    };
                    bool runServices = !runningUsually;
                    bool verbose = initializationInformation.ApplicationConstants.Environment is not Productive;

                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration = new AuthSConfiguration()
                    {
                        RoutesWhereUnauthenticatedAccessIsAllowed = new HashSet<string>() {
                            @$"^/favicon\.ico$",
                            @$"^/API/Other/Resources/APISpecification/*",
                            @$"^/API/Other/Maintenance/Metrics$",
                            @$"^/API/Other/Maintenance/HealthCheck$",
                        },
                    };
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Protocol = new HTTP(HTTP.DefaultPort);
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Domain = domain;
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthenticationServiceSettings = new AuthenticationServiceSettings()
                    {
                        BaseRoleOfAllUser = CodeUnitSpecificConstants.RolenameUsers,
                    };
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuthorizationConfiguration = new AutSRConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.HeaderServiceConfiguration = new HeaderServiceConfiguration();
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.TimeInUTC = false;
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.VideoLength = initializationInformation.ApplicationConstants.Environment is Productive ? TimeSpan.FromMinutes(10) : TimeSpan.FromSeconds(10);
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.AuditLogConfiguration = GRYLogConfiguration.GetCommonConfiguration(AbstractFilePath.FromString("./AuditLog.log"), true);
                    initializationInformation.InitialApplicationConfiguration.ApplicationSpecificConfiguration.ConfigurationForDLoggingMiddleware = new DRequestLoggingConfiguration()
                    {
                        NotLoggedRoutes = new HashSet<string>()
                        {
                            @$"^/favicon\.ico$",
                            @$"^/API/Other/Resources/APISpecification/*",
                            @$"^/API/Other/Maintenance/Metrics$",
                            @$"^/API/Other/Maintenance/HealthCheck$",
                        },
                        MaximalLengthofRequestBodies = 500,
                        MaximalLengthOfResponseBodies = 500,
                    };
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.HostAPISpecificationForInNonDevelopmentEnvironment = true;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.Domain = domain;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.DevelopmentCertificatePasswordHex = GeneralConstants.DevelopmentCertificatePasswordHex;
                    initializationInformation.InitialApplicationConfiguration.ServerConfiguration.DevelopmentCertificatePFXHex = GeneralConstants.DevelopmentCertificatePFXHex;
                    this._Log.Log($"{nameof(initializationInformation.CommandlineParameter.InitialCameraAddresses)}: {initializationInformation.CommandlineParameter.InitialCameraAddresses}", LogLevel.Debug);
                };
                apiServerConfiguration.SetFunctionalInformationAction = (functionalInformation) => //technical initialization for every run
                {
                    functionalInformation.Logger.Log("Run initialization...");
                    IGRYLog logger = functionalInformation.Logger;
                    IAuditLog auditLog = new AuditLog(functionalInformation.InitializationInformation.ApplicationConstants.ExecutionMode.Accept(new GetLoggerVisitor(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuditLogConfiguration, functionalInformation.InitializationInformation.ApplicationConstants.GetLogFolder(), "AuditLog", GRYLog.Create(), logger.Configuration.LogTargets.Where(t => t.LogLevels.Contains(LogLevel.Debug)).Any())));
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuditLog>(auditLog);
                    _Log = logger;
                    bool useDatabase = functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType != null && functionalInformation.InitializationInformation.CommandlineParameter.RealRun && functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType != "Transient";
                    if (useDatabase)
                    {
                        logger.Log($"Run persistent using database \"{functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType}\".", LogLevel.Information);
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService<User>, PersistentAuthenticationService>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, DatabasePersistence>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<User>>(sp => sp.GetRequiredService<IPersistence>());
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IDatabasePersistenceConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration);
                        if (functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType == "PostgreSQL")
                        {
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISQLProvider, SQLProviderPostgreSQL>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IGenericDatabaseInteractor, PostgreSQLDatabaseInteractor>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IConSurvDatabaseInteractor, DatabaseInteractorPostgreSQL>();
                        }
                        else if (functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.DatabasePersistenceConfiguration.DatabaseType == "MariaDB")
                        {
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<ISQLProvider, SQLProviderMariaDB>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IGenericDatabaseInteractor, MariaDBDatabaseInteractor>();
                            functionalInformation.WebApplicationBuilder.Services.AddSingleton<IConSurvDatabaseInteractor, DatabaseInteractorMariaDB>();
                        }
                        else
                        {
                            throw new NotSupportedException("Database not supported. For a list of supported databases see the documentation.");
                        }
                    }
                    else
                    {
                        logger.Log($"Run transient.", LogLevel.Information);
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IPersistence, TransientPersistence>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServiceSettings>(new AuthenticationServiceSettings()
                        {
                            BaseRoleOfAllUser = CodeUnitSpecificConstants.RolenameUsers
                        });
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationService<User>, TransientAuthenticationService<User>>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<ITransientAuthenticationServicePersistence<User>, TransientAuthenticationServicePersistence<User>>();
                        functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServicePersistence<User>>(sp => sp.GetRequiredService<ITransientAuthenticationServicePersistence<User>>());
                    }
                    bool useMockService = runningUsually;
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<ICameraManagementService, CameraManagementService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IRuntimeData, RuntimeData>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IMotionDetectionService, MotionDetectionService>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IGeneralResourceLoader, Misc.GeneralResourceLoader>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationConfiguration>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationConfiguration);
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IAuthenticationServiceSettings>(functionalInformation.PersistedAPIServerConfiguration.ApplicationSpecificConfiguration.AuthenticationServiceSettings);
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
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IInitializationService>(sp => sp.GetRequiredService<IInitializationService<CommandlineParameter>>());
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IExampleDataCreator, ExampleDataCreator>();
                    functionalInformation.WebApplicationBuilder.Services.AddSingleton<IProcessManager, ProcessManager>();
                    functionalInformation.WebApplicationBuilder.Services.AddHealthChecks().AddCheck<HealthCheck>(nameof(HealthCheck));
                };
                apiServerConfiguration.ConfigureWebApplication = (functionalInformationForWebApplication) =>
                {
                    try
                    {
                        this._Constants = apiServerConfiguration;
                        if (runningUsually)
                        {
                            IGeneralLogger logger = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IGeneralLogger>());
                            this._HostApplicationLifetime = functionalInformationForWebApplication.WebApplication.Services.GetService<IHostApplicationLifetime>();
                            this._Log = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IGRYLog>());
                            this._BusinessLogicService = functionalInformationForWebApplication.WebApplication.Services.GetService<IBusinessLogicService>();
                            this._InitializationService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IInitializationService<CommandlineParameter>>());
                            functionalInformationForWebApplication.RunAsync = this.RunAsync;

                            IMetricsService metricsService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IMetricsService>());
                            IMotionDetectionService motionDetectionService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<IMotionDetectionService>());
                            ICameraManagementService cameraManagementService = GUtilities.GetValue(functionalInformationForWebApplication.WebApplication.Services.GetService<ICameraManagementService>());
                            this._Constants = apiServerConfiguration;
                            functionalInformationForWebApplication.PreRun = () =>
                            {
                                //initialize
                                this._InitializationService.Initialize(apiServerConfiguration.CommandlineParameter);
                                metricsService.StartAsync();
                                motionDetectionService.StartAsync();
                                cameraManagementService.StartAsync();
                            };
                            functionalInformationForWebApplication.PostRun = () =>
                            {
                                metricsService.Stop().Wait();
                                motionDetectionService.Stop().Wait();
                                cameraManagementService.Stop().Wait();
                            };
                        }
                    }
                    catch
                    {
                        throw;
                    }
                };
            }, this._Log);
            this.IsRunning = false;
            return result;
        }


        internal void Stop()
        {
            GUtilities.AssertNotNull(this._Constants, nameof(this._Constants)).CancellationTokenSource.Cancel();
        }
    }
}
