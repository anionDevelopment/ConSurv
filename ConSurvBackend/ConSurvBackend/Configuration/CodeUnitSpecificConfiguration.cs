using GRYLibrary.Core.APIServer.CommonRoutes;
using GRYLibrary.Core.APIServer.MaintenanceRoutes;
using GRYLibrary.Core.APIServer.Mid.M05DLog;
using GRYLibrary.Core.APIServer.MidT.Aut;
using GRYLibrary.Core.APIServer.MidT.Auth;
using GRYLibrary.Core.APIServer.MidT.RLog;
using GRYLibrary.Core.APIServer.Services.CredC;

namespace ConSurvBackend.Core.Configuration
{
    public class CodeUnitSpecificConfiguration : ISupportRequestLoggingMiddleware, ISupportAuthenticationMiddleware, ISupportAuthorizationMiddleware
    {
        public ICommonRoutesInformation CommonRoutesInformation { get; set; }
        public IMaintenanceRoutesInformation MaintenanceRoutesInformation { get; set; }
        public ICameraSchedulerServiceSettings SomeBackgroundServiceSettings { get; set; }
        public IDRequestLoggingConfiguration ConfigurationForDLoggingMiddleware { get; set; }
        public IRequestLoggingConfiguration ConfigurationForLoggingMiddleware { get { return this.ConfigurationForDLoggingMiddleware; } }
        public string DatabaseConnectionString { get; set; }
        public IAuthorizationConfiguration AuthorizationConfiguration { get; set; }
        public IAuthorizationConfiguration ConfigurationForAuthorizationMiddleware { get { return this.AuthorizationConfiguration; } }
        public IAuthenticationConfiguration AuthenticationConfiguration { get; set; }
        public IAuthenticationConfiguration ConfigurationForAuthenticationMiddleware { get { return this.AuthenticationConfiguration; } }
        public ICameraSchedulerServiceSettings CameraSchedulerServiceSettings { get; set; }
        public ICookieServiceConfiguration CookieServiceConfiguration { get; set; }
    }
}
