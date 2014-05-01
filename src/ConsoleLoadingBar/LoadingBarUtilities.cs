using System;
using System.IO;

namespace ConsoleLoadingBar
{
    public static class LoadingBarUtilities
    {
        public static bool IsConsoleApp()
        {
            if (!Environment.UserInteractive)
                return false;

            return Console.OpenStandardInput(1) != Stream.Null;
        }
    }
}
