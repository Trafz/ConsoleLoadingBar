using System;
using ConsoleLoadingBar.Enums;

namespace ConsoleLoadingBar.Helpers
{
    public static class LoadingBarBehaviourHelper
    {
        public static LoadingBarBehaviour GetAppSettingsForMultipleBars()
        {
            const string prefix = "ConsoleLoadingBar.Behaviour.Multiple.";

            var behaviour = LoadingBarBehaviour.Null;

            string[] enumNames = Enum.GetNames(typeof(LoadingBarBehaviour));
            Array a = Enum.GetValues(typeof(LoadingBarBehaviour));
            int counter = 0;
            foreach (string enumName in enumNames)
            {
                if (LoadingBarAppSettingsHelper.GetSettingAsBool(prefix + enumName))
                    behaviour = behaviour | (LoadingBarBehaviour)a.GetValue(counter);

                counter++;
            }

            return behaviour;
        }
    }
}
