using System;

namespace ConsoleLoadingBar.Enums
{
    [Flags]
    public enum LoadingBarBehavior
    {
        Default = 0,
        ClearWhenHundredPercentIsHit = 1 << 0,
        OnlyUpdateMessageOnPercentageChange = 1 << 1,
        AppendEtaToMessage = 1 << 2
    }
}
