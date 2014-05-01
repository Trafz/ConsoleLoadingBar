using System;
using ConsoleLoadingBar.Enums;
using JetBrains.Annotations;

namespace ConsoleLoadingBar
{
    public class SingleLoadingBar : IDisposable
    {
        // public fields
        public const char DefaultChar = '\u2592';
        public readonly int Total; // TODO: Allow this to be changed. Remember to update the percentages etc

        // private fields
        [NotNull]
        private readonly object _syncRegulate = new object();
        [NotNull]
        private readonly object _syncObject = new object();
        [NotNull]
        private readonly ConsoleOperator _consoleOperator;

        private readonly int _alternateGetBackToLine = -1;
        private readonly ConsoleColor _chosenColor;
        private readonly char _chosenChar;
        private readonly string _message;

        private int _prevGetStartLocation = -1;
        private LoadingBarBehavior _behavior;
        private int _alternateGetBackToRow;
        private int _previousPercentage;
        private int _current;
        private bool _hasCleared;
        private int _regulate;


        public SingleLoadingBar(
            int total,
            char progressChar = DefaultChar,
            ConsoleColor color = ConsoleColor.Blue,
            string message = null,
            ConsoleOperator consoleOperator = null,
            int alternateGetBackToLine = -1,
            int alternateGetBackToRow = -1)
        {
            Total = total;
            _chosenChar = progressChar;
            _chosenColor = color;
            _message = message;
            _alternateGetBackToLine = alternateGetBackToLine;
            _alternateGetBackToRow = alternateGetBackToRow;

            LocationLine = -1;
            LocationRow = -1;

            _consoleOperator = consoleOperator ?? new ConsoleOperator();
            if (_consoleOperator.IsConsoleApp)
                GetInitialData();
        }


        public int LocationLine { get; private set; }
        public int LocationRow { get; private set; }

        public LoadingBarBehavior Behavior
        {
            // ReSharper disable once UnusedMember.Global
            get { return _behavior; }
            set { _behavior = value; }
        }

        public int AlternateGetBackToRow
        {
            get { return _alternateGetBackToRow; }
            set { _alternateGetBackToRow = value; }
        }


        public void Update()
        {
            lock (_syncObject)
                _current++;

            Update(CalculatePercentage(_current, Total));
        }

        public void Update(string message)
        {
            lock (_syncObject)
                _current++;

            Update(CalculatePercentage(_current, Total));
        }

        public void Update(int percentage, string message = null)
        {
            if (_consoleOperator.IsConsoleApp == false)
                return;
            if (percentage != 0 && percentage == _previousPercentage && _behavior.HasFlag(LoadingBarBehavior.OnlyUpdateMessageOnPercentageChange))
                return;

            lock (_syncObject)
            {
                Console.CursorVisible = false;

                _previousPercentage = percentage;

                Console.CursorTop = GetStartLocation();
                if (_prevGetStartLocation == -1)
                    _prevGetStartLocation = Console.CursorTop;
                lock (_syncRegulate)
                {
                    _consoleOperator.GotoNextLine(ref _regulate);
                }

                SaveTheCurrentLineIfNeeded();

                if (percentage == 100 && _behavior.HasFlag(LoadingBarBehavior.ClearWhenHundredPercentIsHit) && !_hasCleared)
                {
                    int rowBefore = Console.CursorTop;
                    lock (_syncRegulate)
                    {
                        _consoleOperator.ClearLine(ref _regulate);
                        if (rowBefore == Console.CursorTop)
                            _consoleOperator.GotoNextLine(ref _regulate);
                        _consoleOperator.ClearLine(ref _regulate, goingLast: true);
                    }

                    _hasCleared = true;
                }
                else
                {
                    int line, row;
                    GetCurrentPosition(out line, out row);

                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = _chosenColor;

                    Console.CursorLeft = 0;
                    WriteProgressBar(percentage);
                    WriteConsoleMessage(percentage, message);
                    Console.ForegroundColor = originalColor;
                }

                Console.CursorTop = _alternateGetBackToRow == -1 ? GetStartLocation() : _alternateGetBackToRow;
                Console.CursorLeft = _alternateGetBackToLine == -1 ? LocationLine : _alternateGetBackToLine;
                Console.CursorVisible = true;
            }
        }



        public void WriteProgressBar(int percentage)
        {
            int width = Console.WindowWidth - 1;
            var newWidth = (int)((width * percentage) / 100d);
            if (newWidth > width)
                newWidth = width;

            string progressBar = new string(_chosenChar, newWidth) + new string(' ', width - newWidth);
            Console.WriteLine(progressBar);
        }

        public void WriteConsoleMessage(int percentage, string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.Write(message);
                return;
            }

            string consoleMessage = string.IsNullOrWhiteSpace(_message)
                ? string.Format("{0} out of {1} ({2}%)", _current, Total, percentage)
                : string.Format("{0} - {1}%", _message, percentage);
            Console.Write(consoleMessage);
        }

        public int GetStartLocation()
        {
            return LocationRow - _regulate;
        }

        public void UpdateRegular(int difference)
        {
            if (difference == 0)
                return;

            lock (_syncRegulate)
            {
                _regulate = _regulate + difference;
            }
        }

        public void Clear()
        {
            if (!_consoleOperator.IsConsoleApp)
                return;

            lock (_syncObject)
            {
                Console.CursorTop = GetStartLocation();
                lock (_syncRegulate)
                {
                    _consoleOperator.GotoNextLine(ref _regulate);
                    Console.CursorLeft = 0;
                    _consoleOperator.ClearLine(ref _regulate);
                    _consoleOperator.ClearLine(ref _regulate, goingLast: true);
                }

                Console.CursorTop = _alternateGetBackToRow == -1 ? GetStartLocation() : _alternateGetBackToRow;
                Console.CursorLeft = LocationLine;
            }

            Console.CursorVisible = true;
        }


        public int CalculatePercentage(int current, int total)
        {
            return total == 0 ? 0 : Convert.ToInt32(Math.Ceiling(current / (decimal)total * 100));
        }

        public void GetInitialData()
        {
            LocationRow = Console.CursorTop;
            LocationLine = Console.CursorLeft;
        }

        public void Dispose()
        {
            Clear();

            // TODO: Research what a proper Dispose() should include and implement it
            // I recall seeing these quite often. Google it: GC.SuppressFinalize(this);
        }

        private void GetCurrentPosition(out int lineLocation, out int rowFromTop)
        {
            rowFromTop = Console.CursorTop;
            lineLocation = Console.CursorLeft;
        }

        private void SaveTheCurrentLineIfNeeded()
        {
            /* So... I wanted to detect if I wrote the line or something else did.
             * If something else did, then I'd copy the line, remove the _chosenChars and save it.
             * When the bar would have run to completion, we'd rewrite the line as we had saved it.
             * Because that's probably how it was meant to look like */
        }
    }
}
