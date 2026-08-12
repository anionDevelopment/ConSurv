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

        /// <summary>Key of the user-setting which holds the color-scheme the user chose.</summary>
        public const string UserSettingKeyTheme = "Theme";

        /// <summary>The color-scheme of a user who did not choose one. It follows the setting of the operating-system of that user.</summary>
        public const string ThemeSystem = "system";

        /// <summary>The color-scheme which is always light, regardless of the setting of the operating-system.</summary>
        public const string ThemeLight = "light";

        /// <summary>The color-scheme which is always dark, regardless of the setting of the operating-system.</summary>
        public const string ThemeDark = "dark";

        /// <summary>All values which are accepted as color-scheme of a user.</summary>
        public static readonly System.Collections.Generic.ISet<string> Themes = new System.Collections.Generic.HashSet<string>() { ThemeSystem, ThemeLight, ThemeDark };
    }
}
