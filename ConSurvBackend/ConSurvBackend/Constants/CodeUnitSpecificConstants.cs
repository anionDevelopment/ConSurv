namespace ConSurvBackend.Core.Constants
{
    public class CodeUnitSpecificConstants
    {
        public const string ProductName = "ConSurv";
        public const string UserNameAdmin = "admin";
        public const string UserGroupAdmin = "Adminstrators";
        public const string UserGroupCameraManagers = "CameraManagers";
        public const string UserGroupUser= "Users";
        public const string AvailableCamerasRatioMeterName = "AvailableCamerasRatio";
        public const string WebControllerRoute = "Web";
        public const string BusinessMetricsPrefix = $"{GeneralConstants.CodeUnitName}_Business_";
        public const string MetricsNameAvailableCamerasRate = $"{BusinessMetricsPrefix}_AvailableCamerasRate";
    }
}
