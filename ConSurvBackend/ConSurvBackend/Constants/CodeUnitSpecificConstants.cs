namespace ConSurvBackend.Core.Constants
{
    public class CodeUnitSpecificConstants
    {
        public const string ProductName = "ConSurv";
        public const string UsernameAdmin = "admin";
        public const string RolenameAdmins = "Adminstrators";
        public const string RolenameModerators = "CameraManagers";
        public const string RolenameUsers = "Users";
        public const string WebControllerRoute = "Web";
        public const string BusinessMetricsPrefix = $"{GeneralConstants.CodeUnitName}_Business";
        public const string MetricsNameAvailableCamerasRate = $"{BusinessMetricsPrefix}_AvailableCamerasRate";
    }
}
