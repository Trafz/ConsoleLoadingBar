using System.Configuration;
using ConsoleLoadingBar.Enums;
using JetBrains.Annotations;

namespace ConsoleLoadingBar.Helpers
{
    public static class LoadingBarAppSettingsHelper
    {
        public static bool AllowLineBreaks()
        {
            return GetSettingAsBool(LoadingBarConstAppSettingsNames.AllowLineBreaks);
        }

        [ContractAnnotation("null => false")]
        public static bool GetSettingAsBool([CanBeNull] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            string setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(setting))
                return false;

            if (setting.Trim().ToLower() == "true")
                return true;

            return false;
        }
    }
}
