using System;

namespace ConsoleLoadingBar.Enums
{
    [Flags]
    public enum LoadingBarBehaviour
    {
        Null = 0,
        ClearWhenHundredPercentIsHit = 1 << 0
    }
}
