﻿using System;
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
            ConsoleOperator consoleOperator = null,
            int alternateGetBackToLine = -1)
        {
            Total = total;
            _chosenChar = progressChar;
            _chosenColor = color;
            _message = message;
            _alternateGetBackToLine = alternateGetBackToLine;

            _consoleOperator = consoleOperator ?? new ConsoleOperator();
            if (_consoleOperator.IsConsoleApp)
                GetInitialData();
        }

        public const char DefaultChar = '\u2592';
        public readonly int Total;
        public int LocationLine = -1;
        public int LocationRow = -1;
        public int AlternateGetBackToRow = -1;
        public LoadingBarBehaviour Behaviour = 0;
        public int Regulate;

        private readonly object _syncObject = new object();
        private readonly object _syncRegulate = new object();
        private int _prevGetStartLocation = -1;
        private readonly int _alternateGetBackToLine = -1;
        private readonly char _chosenChar;
        private readonly ConsoleColor _chosenColor;
        private readonly string _message;
        private readonly ConsoleOperator _consoleOperator;
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
            if (_consoleOperator.IsConsoleApp == false)
                return;
            if (percentage != 0 && percentage == _previousPercentage)
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
                    _consoleOperator.GotoNextLine(ref Regulate);
                }

                SaveTheCurrentLineIfNeeded();

                if (percentage == 100 && Behaviour.HasFlag(LoadingBarBehaviour.ClearWhenHundredPercentIsHit) && !_hasCleared)
                {
                    int rowBefore = Console.CursorTop;
                    lock (_syncRegulate)
                    {
                        _consoleOperator.ClearLine(ref Regulate);
                        if (rowBefore == Console.CursorTop)
                            _consoleOperator.GotoNextLine(ref Regulate);
                        _consoleOperator.ClearLine(ref Regulate, goingLast: true);
                    }
                    _hasCleared = true;
                }
                else
                {
                    int line, row;
                    GetCurrentPosition(out line, out row);

                    string consoleMessage = string.IsNullOrWhiteSpace(_message)
                        ? string.Format("{0} out of {1} ({2}%)", _current, Total, percentage)
                        : string.Format("{0} - {1}%", _message, percentage);

                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = _chosenColor;
                    int width = Console.WindowWidth - 1;
                    var newWidth = (int)((width * percentage) / 100d);
                    if (newWidth > width)
                        newWidth = width;
                    string progBar = new string(_chosenChar, newWidth) +
                                     new string(' ', width - newWidth);
                    Console.CursorLeft = 0;
                    Console.WriteLine(progBar);
                    Console.Write(consoleMessage);
                    Console.ForegroundColor = originalColor;
                }

                Console.CursorTop = AlternateGetBackToRow == -1 ? GetStartLocation() : AlternateGetBackToRow;
                Console.CursorLeft = _alternateGetBackToLine == -1 ? LocationLine : _alternateGetBackToLine;
                Console.CursorVisible = true;
            }
        }


        public int GetStartLocation()
        {
            return LocationRow - Regulate;
        }

        private void SaveTheCurrentLineIfNeeded()
        {
            /* So... I wanted to detect if I wrote the line or something else did.
             * If something else did, then I'd copy the line, remove the _chosenChars and save it.
             * When the bar would have run to completion, we'd rewrite the line as we had saved it.
             * Because that's probably how it was meant to look like */
        }

        public void UpdateRegular(int difference)
        {
            if (difference == 0)
                return;

            lock (_syncRegulate)
            {
                Regulate = Regulate + difference;
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
                    _consoleOperator.GotoNextLine(ref Regulate);
                    Console.CursorLeft = 0;
                    _consoleOperator.ClearLine(ref Regulate);
                    _consoleOperator.ClearLine(ref Regulate, goingLast: true);
                }

                Console.CursorTop = AlternateGetBackToRow == -1 ? GetStartLocation() : AlternateGetBackToRow;
                Console.CursorLeft = LocationLine;
            }

            Console.CursorVisible = true;
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


        public void Dispose()
        {
            Clear();
            // TODO: Research what a proper Dispose() should include and implement it
            // I recall seeing these quite often. Google it: GC.SuppressFinalize(this);
        }
    }
}