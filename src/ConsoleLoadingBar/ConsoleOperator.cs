using System;
using ConsoleLoadingBar.Core.Helpers;

namespace ConsoleLoadingBar.Core
{
    public class ConsoleOperator
    {
        private readonly bool _allowLineBreaks;

        public ConsoleOperator()
        {
            _allowLineBreaks = LoadingBarAppSettingsHelper.AllowLineBreaks();
            IsConsoleApp = LoadingBarUtilities.IsConsoleApp();
        }


        public bool IsConsoleApp { get; private set; }


        public void GotoNextLine(ref int lockedRegulateProperty)
        {
            if (Console.BufferHeight > Console.CursorTop + 2)
                Console.CursorTop = Console.CursorTop + 1;
            else
            {
                lockedRegulateProperty += 2;
                Console.Write(Environment.NewLine);
            }
        }

        public void ClearLine(ref int lockedRegulateProperty, bool goingLast = false)
        {
            if (IsConsoleApp == false)
                return;

            Console.CursorLeft = 0;

            if (_allowLineBreaks && Console.CursorTop == Console.BufferHeight - 1)
            {
                ClearLineAndEndOnNext();
                if (goingLast == false)
                    return;

                lockedRegulateProperty++;
                return;
            }

            if (Console.CursorTop != Console.BufferHeight - 1)
                ClearLineAndEndOnNext();
            else
                AlmostClearLineButEndAtTheLastOne();
        }

        public void GetInitialData(out int row, out int line)
        {
            row = Console.CursorTop;
            line = Console.CursorLeft;
        }


        private static void ClearLineAndEndOnNext()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(' ');
        }

        private static void AlmostClearLineButEndAtTheLastOne()
        {
            for (int i = 0; i < Console.WindowWidth - 1; i++)
                Console.Write(' ');
        }
    }
}
