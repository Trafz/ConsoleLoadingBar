using System;

namespace ConsoleLoadingBar.Core.Enums
{
    [Flags]
    public enum LoadingBarBehavior
    {
        Default = 0,
        ClearWhenHundredPercentIsHit = 1 << 0,
        OnlyUpdateMessageOnPercentageChange = 1 << 1
    }
}
