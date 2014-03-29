using System;
using ConsoleLoadingBar.Core.Enums;

namespace ConsoleLoadingBar.Core.Helpers
{
    public static class LoadingBarBehaviorHelper
    {
        public static LoadingBarBehavior GetAppSettingsForMultipleBars()
        {
            const string prefix = "ConsoleLoadingBar.Behavior.Multiple.";

            var behavior = LoadingBarBehavior.Default;

            string[] enumNames = Enum.GetNames(typeof(LoadingBarBehavior));
            Array a = Enum.GetValues(typeof(LoadingBarBehavior));

            for (int i = 0; i < enumNames.Length; i++)
            {
                if (!LoadingBarAppSettingsHelper.GetSettingAsBool(prefix + enumNames[i]))
                    continue;

                var o = a.GetValue(i);
                if (o is LoadingBarBehavior)
                    behavior = behavior | (LoadingBarBehavior)o;
            }

            return behavior;
        }
    }
}
