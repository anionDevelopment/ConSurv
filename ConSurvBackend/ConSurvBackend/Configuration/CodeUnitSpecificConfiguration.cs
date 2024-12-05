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
using System;

namespace ConSurvBackend.Core.Configuration
{
    public class CodeUnitSpecificConfiguration : ISupportRequestLoggingMiddleware, ISupportExceptionManagerMiddleware, ISupportAuthenticationMiddleware, ISupportAuthorizationMiddleware
    {
        public bool RegistrationIsEnabled { get; set; }
        public bool LoginIsEnabled { get; set; }
        public string DatabaseConnectionString { get; set; }
        public ICommonRoutesInformation CommonRoutesInformation { get; set; }
        public IMaintenanceRoutesInformation MaintenanceRoutesInformation { get; set; }
        public IDRequestLoggingConfiguration ConfigurationForDLoggingMiddleware { get; set; }
        public IRequestLoggingConfiguration ConfigurationForLoggingMiddleware { get { return this.ConfigurationForDLoggingMiddleware; } }
        public IAutSRConfiguration AuthorizationConfiguration { get; set; }
        public IAuthorizationConfiguration ConfigurationForAuthorizationMiddleware { get { return this.AuthorizationConfiguration; } }
        public IAuthSConfiguration AuthenticationConfiguration { get; set; }
        public IAuthenticationConfiguration ConfigurationForAuthenticationMiddleware { get { return this.AuthenticationConfiguration; } }
        public string TargetFolder { get; set; }
        public TimeSpan VideoLength { get; set; }
        public bool TimeInUTC { get; set; }
        public IExceptionManagerConfiguration ConfigurationForExceptionManagerMiddleware { get; set; }
        public IHeaderServiceConfiguration HeaderServiceConfiguration { get;  set; }
    }
}
