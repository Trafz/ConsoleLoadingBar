using System.Configuration;
using ConsoleLoadingBar.Enums;

namespace ConsoleLoadingBar.Helpers
{
    public static class LoadingBarAppSettingsHelper
    {
        public static bool AllowLineBreaks()
        {
            return GetSettingAsBool(LoadingBarConstAppSettingsNames.AllowLineBreaks);
        }

        public static bool GetSettingAsBool(string key)
        {
            string setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(setting))
                return false;

            if (setting.Trim().ToLower() == "true")
                return true;

            return false;
        }
    }
}
