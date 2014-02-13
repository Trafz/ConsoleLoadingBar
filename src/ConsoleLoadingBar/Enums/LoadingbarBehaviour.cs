using System;

namespace ConsoleLoadingBar.Enums
{
    [Flags]
    public enum LoadingbarBehaviour
    {
        ClearWhenHundredPercentIsHit = 1 << 0
    }
}
