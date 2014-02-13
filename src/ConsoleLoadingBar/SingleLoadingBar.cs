using System;
using ConsoleLoadingBar.Enums;

namespace ConsoleLoadingBar
{
    public class SingleLoadingBar : IDisposable
    {
        public SingleLoadingBar(
            int total,
            char progressChar = DefaultChar,
            ConsoleColor color = ConsoleColor.Blue,
            string message = null,
            LoadingbarBehaviour behaviour = 0)
        {
            IsConsoleApp = LoadingBarUtilities.IsConsoleApp();
            if (IsConsoleApp)
                GetInitialData();

            Total = total;
            _chosenChar = progressChar;
            _chosenColor = color;
            _message = message;
            Behaviour = behaviour;
        }

        public const char DefaultChar = '\u2592';
        public readonly int Total;
        public readonly bool IsConsoleApp;
        public int LocationLine = -1;
        public int LocationRow = -1;
        public LoadingbarBehaviour Behaviour;

        private readonly object _syncObject = new object();
        private readonly char _chosenChar;
        private readonly ConsoleColor _chosenColor;
        private readonly string _message;
        private int _previousPercentage;
        private int _current;
        private bool _hasCleared;


        public void Update()
        {
            lock (_syncObject)
                _current++;

            Update(CalculatePercentage(_current, Total));
        }
        public void Update(int percentage)
        {
            if (IsConsoleApp == false)
                return;
            if (percentage != 0 && percentage == _previousPercentage)
                return;

            lock (_syncObject)
            {
                _previousPercentage = percentage;
                Console.CursorTop = LocationRow + 1;
                SaveTheCurrentLineIfNeeded();

                if (percentage == 100 && Behaviour.HasFlag(LoadingbarBehaviour.ClearWhenHundredPercentIsHit) && !_hasCleared)
                {
                    int rowBefore = Console.CursorTop;
                    ClearLine();
                    if (rowBefore == Console.CursorTop)
                        Console.Write(Environment.NewLine);
                    ClearLine();
                    _hasCleared = true;
                }
                else
                {
                    int line, row;
                    GetCurrentPosition(out line, out row);

                    string consoleMessage = string.IsNullOrWhiteSpace(_message)
                        ? string.Format("{0} out of {1} ({2}%)", _current, Total, percentage)
                        : string.Format("{0} - {1}%", _message, percentage);

                    Console.CursorLeft = 0;

                    Console.CursorVisible = false;
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = _chosenColor;
                    int width = Console.WindowWidth - 1;
                    var newWidth = (int)((width * percentage) / 100d);
                    if (newWidth > width)
                        newWidth = width;
                    string progBar = new string(_chosenChar, newWidth) +
                                     new string(' ', width - newWidth);
                    Console.WriteLine(progBar);
                    Console.WriteLine(consoleMessage);
                    Console.ForegroundColor = originalColor;
                }

                Console.CursorTop = LocationRow;
                Console.CursorLeft = LocationLine;
            }
        }

        private void SaveTheCurrentLineIfNeeded()
        {
            /* So... I wanted to detect if I wrote the line or something else did.
             * If something else did, then I'd copy the line, remove the _chosenChars and save it.
             * When the bar would have run to completion, we'd rewrite the line as we had saved it.
             * Because that's probably how it was meant to look like */
        }

        public void Clear()
        {
            if (!IsConsoleApp)
                return;

            lock (_syncObject)
            {
                Console.CursorVisible = true;

                Console.CursorTop = LocationRow + 1;
                Console.CursorLeft = 0;
                ClearLine();
                ClearLine();

                Console.CursorTop = LocationRow;
                Console.CursorLeft = LocationLine;
            }
        }


        public int CalculatePercentage(int current, int total)
        {
            return total == 0 ? 0 : Convert.ToInt32(Math.Ceiling((current / (decimal)total * 100)));
        }
        public int CalculateCurrent(int percentage)
        {
            return Convert.ToInt32(Math.Round((percentage / (decimal)100 * Total)));
        }

        public void GetInitialData()
        {
            LocationRow = Console.CursorTop;
            LocationLine = Console.CursorLeft;
        }
        private void GetCurrentPosition(out int lineLocation, out int rowFromTop)
        {
            rowFromTop = Console.CursorTop;
            lineLocation = Console.CursorLeft;
        }
        private void ClearLine()
        {
            if (IsConsoleApp == false)
                return;

            Console.CursorLeft = 0;
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(' ');
        }

        public void Dispose()
        {
            Clear();
            // TODO: Research what a proper Dispose() should include and implement it
            // I recall seeing these quite often. Google it: GC.SuppressFinalize(this);
        }
    }
}