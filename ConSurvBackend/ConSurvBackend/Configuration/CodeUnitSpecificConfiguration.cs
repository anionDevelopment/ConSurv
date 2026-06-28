using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.MaintenanceRoutes;
using GRYLibrary.Core.APIServer.Mid.AuthS;
using GRYLibrary.Core.APIServer.Mid.AutS;
using GRYLibrary.Core.APIServer.Mid.M05DLog;
using GRYLibrary.Core.APIServer.MidT.Aut;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.MidT.Exception;
using GRYLibrary.Core.APIServer.MidT.RLog;
using GRYLibrary.Core.APIServer.Services.CredH;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.Configuration
{
    public class CodeUnitSpecificConfiguration : ISupportRequestLoggingMiddleware, ISupportExceptionManagerMiddleware, ISupportAuthenticationMiddleware, ISupportAuthorizationMiddleware
    {
        public bool RegistrationIsEnabled { get; set; }
        public bool LoginIsEnabled { get; set; }
        /// <summary>
        /// When 2 consecutive images from a camera have a similarity above this threshold, the camera is considered to be currently showing no motion. The similarity is calculated by OpenCV's AverageHash algorithm and is a value between 0 and 1, where 1 means identical images and 0 means completely different images.
        /// </summary>
        public double MotionDetectionThreshold { get; set; } = 0.95;
        public TimeSpan VideoRetentionPeriod { get; set; } = TimeSpan.FromDays(7 * 5);
        public TimeSpan VideoLength { get; set; }
        public bool TimeInUTC { get; set; }
        public ICommonRoutesInformation CommonRoutesInformation { get; set; }
        public IMaintenanceRoutesInformation MaintenanceRoutesInformation { get; set; }
        public IDRequestLoggingConfiguration ConfigurationForDLoggingMiddleware { get; set; }
        public IRequestLoggingConfiguration ConfigurationForLoggingMiddleware { get { return this.ConfigurationForDLoggingMiddleware; } }
        public IAutSRConfiguration AuthorizationConfiguration { get; set; }
        public IAuthorizationConfiguration ConfigurationForAuthorizationMiddleware { get { return this.AuthorizationConfiguration; } }
        public IAuthSConfiguration AuthenticationConfiguration { get; set; }
        public IAuthenticationConfiguration ConfigurationForAuthenticationMiddleware { get { return this.AuthenticationConfiguration; } }
        public IExceptionManagerConfiguration ConfigurationForExceptionManagerMiddleware { get; set; }
        public IHeaderServiceConfiguration HeaderServiceConfiguration { get; set; }
        public IGRYLogConfiguration AuditLogConfiguration { get; set; }
        public IGRYLogConfiguration HousekeepingServiceLogConfiguration { get; set; }
        public IGRYLogConfiguration CameraManagementServiceLogConfiguration { get; set; }
        public IGRYLogConfiguration MetricsServiceLogConfiguration { get; set; }
        public IGRYLogConfiguration MotionDetectionServiceLogConfiguration { get; set; }
        public IDatabasePersistenceConfiguration DatabasePersistenceConfiguration { get; set; }
        public IAuthenticationServiceSettings AuthenticationServiceSettings { get; set; }
    }
}
