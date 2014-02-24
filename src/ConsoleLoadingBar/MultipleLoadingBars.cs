using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConsoleLoadingBar.Enums;
using ConsoleLoadingBar.Helpers;

namespace ConsoleLoadingBar
{
    public class MultipleConsoleLoadingBars : IDisposable
    {
        public MultipleConsoleLoadingBars()
        {
            _locationRow = Console.CursorTop;
            _locationLine = Console.CursorLeft;

            _consoleOperator = new ConsoleOperator();

            ResetColors();

            _behaviour = LoadingBarBehaviourHelper.GetAppSettingsForMultipleBars();
        }

        private readonly List<SingleLoadingBar> _loadingBars = new List<SingleLoadingBar>();
        public IReadOnlyList<SingleLoadingBar> LoadingBars
        {
            get { return new List<SingleLoadingBar>(_loadingBars); }
        }


        public ConcurrentBag<ConsoleColor> Colors { get; set; }
        public void ResetColors()
        {
            Colors = new ConcurrentBag<ConsoleColor>
                {
                    ConsoleColor.Green,
                    ConsoleColor.Red,
                    ConsoleColor.Yellow,
                    ConsoleColor.Blue,
                    ConsoleColor.White
                };
        }
        public ConsoleColor PickNextColor()
        {
            ConsoleColor color;
            bool didTake = Colors.TryTake(out color);
            if (didTake == false)
            {
                lock (Colors)
                {
                    if (Colors.Count == 0)
                        ResetColors();

                    Colors.TryTake(out color);
                }
            }

            return color;
        }


        public SingleLoadingBar CreateNewLoadingBar(int total, ConsoleColor color = ConsoleColor.Black, string message = null)
        {
            if (color == ConsoleColor.Black)
                color = PickNextColor();

            if (Console.CursorTop != GetStartLocation())
                throw new Exception();

            MakeSureWeHaveSpaceForNewLoadingBar();

            GotoLineAboveNextBar();
            var loadingBar = new SingleLoadingBar(total, '\x2592', color, message, _consoleOperator, _locationLine)
            {
                AlternateGetBackToRow = GetStartLocation(),
                //Behaviour = LoadingBarBehaviour.ClearWhenHundredPercentIsHit
                Behaviour = _behaviour
            };
            AddLoadingBar(loadingBar);

            int startLocation = GetStartLocation();
            int asd = Console.BufferHeight - _loadingBars.Count * 2 - 1;
            if (startLocation >= asd)
            {
                for (int i = 0; i < _loadingBars.Count; i++)
                {
                    int a = _loadingBars[i].AlternateGetBackToRow;
                    int b = GetStartLocation();
                    if (a != b)
                    {
                        _loadingBars[i].AlternateGetBackToRow = b;
                    }
                }
            }
            /* You do NOT need a GotoStart() here.
             * If you want to place one, then you've made an error somewhere
             * GotoStart(); */

            return loadingBar;
        }

        private void GotoLineAboveNextBar()
        {
            int row = GetStartLocation() + _loadingBars.Count * 2;
            if (row >= Console.BufferHeight)
                throw new Exception("Need more space!");

            Console.CursorTop = row;
        }
        private void MakeSureWeHaveSpaceForNewLoadingBar()
        {
            int asd = GetStartLocation() + (_loadingBars.Count + 1) * 2;
            int regulate = _regulate; // TODO: Check if this if-else could be (_regulate != 0)
            if (asd < Console.BufferHeight) // This one is for TestCase1
                return;
            if (asd > Console.BufferHeight + 1) // This one is for TestCase2
                return;

            Console.CursorTop = Console.BufferHeight - 1;
            lock (_syncRegulate)
            {
                int total = asd - Console.BufferHeight + 1;
                for (int i = 0; i < total; i++)
                {
                    _consoleOperator.GotoNextLine(ref _regulate);
                    _regulate--;
                }

                foreach (SingleLoadingBar loadingBar in _loadingBars)
                {
                    loadingBar.UpdateRegular(total);
                }
            }

            GotoStart();
        }

        public void AddLoadingBar(SingleLoadingBar loadingBar)
        {
            loadingBar.Update(0);
            _loadingBars.Add(loadingBar);
        }


        public void CleanUp()
        {
            GotoStart();
            Console.CursorTop++;

            lock (_syncRegulate)
            {
                bool goingLast = false;

                for (int i = 0; i < _loadingBars.Count * 2; i++)
                {
                    _consoleOperator.ClearLine(ref _regulate, goingLast);

                    if (i == _loadingBars.Count * 2 - 2)
                        goingLast = true;
                }
            }

            GotoStart();
        }

        public void Dispose()
        {
            if (LoadingBarAppSettingsHelper.AllowLineBreaks() && _regulate != 0)
            {
                lock (_syncRegulate)
                {
                    _regulate++;
                }
            }

            CleanUp();
        }

        private void GotoStart()
        {
            Console.CursorTop = GetStartLocation();
            Console.CursorLeft = _locationLine;
        }

        public int GetStartLocation()
        {
            return _locationRow - _regulate;
        }


        private readonly object _syncRegulate = new object();
        private readonly int _locationRow;
        private readonly int _locationLine;
        private readonly ConsoleOperator _consoleOperator;
        private int _regulate;
        private LoadingBarBehaviour _behaviour;
    }
}
