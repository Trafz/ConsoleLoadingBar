using System;

namespace ConsoleLoadingBar.Enums
{
    [Flags]
    public enum LoadingBarBehaviour
    {
        Default = 0,
        ClearWhenHundredPercentIsHit = 1 << 0,
        OnlyUpdateMessageOnPercentageChange = 1 << 1
    }
}