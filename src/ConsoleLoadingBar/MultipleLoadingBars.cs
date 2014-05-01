using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConsoleLoadingBar.Enums;
using ConsoleLoadingBar.Helpers;
using JetBrains.Annotations;

namespace ConsoleLoadingBar
{
    public class MultipleConsoleLoadingBars : IDisposable
    {
        [NotNull]
        private readonly ConsoleOperator _consoleOperator;
        [NotNull]
        private readonly List<SingleLoadingBar> _loadingBars = new List<SingleLoadingBar>();
        [NotNull]
        private readonly object _syncRegulate = new object();

        private readonly LoadingBarBehavior _behavior;
        private readonly int _locationLine;
        private readonly int _locationRow;

        [NotNull]
        private ConcurrentBag<ConsoleColor> _colors = new ConcurrentBag<ConsoleColor>();

        private int _regulate;


        public MultipleConsoleLoadingBars()
        {
            _consoleOperator = new ConsoleOperator();
            if (_consoleOperator.IsConsoleApp)
            {
                _locationRow = Console.CursorTop;
                _locationLine = Console.CursorLeft;
            }

            ResetColors();

            _behavior = LoadingBarBehaviorHelper.GetAppSettingsForMultipleBars();
        }

        [NotNull]
        public IReadOnlyList<SingleLoadingBar> LoadingBars
        {
            get { return new List<SingleLoadingBar>(_loadingBars); }
        }


        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public ConcurrentBag<ConsoleColor> Colors
        {
            get
            {
                return _colors;
            }

            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // ReSharper disable once HeuristicUnreachableCode
                if (value == null)
                    return;

                _colors = value;
            }
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

        // ReSharper disable once MemberCanBePrivate.Global
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


        // ReSharper disable once UnusedMethodReturnValue.Global
        [CanBeNull]
        public SingleLoadingBar CreateNewLoadingBar(int total, ConsoleColor color = ConsoleColor.Black, string message = null)
        {
            if (color == ConsoleColor.Black)
                color = PickNextColor();

            if (_consoleOperator.IsConsoleApp && Console.CursorTop != GetStartLocation())
                throw new Exception();

            MakeSureWeHaveSpaceForNewLoadingBar();

            GotoLineAboveNextBar();
            var loadingBar = new SingleLoadingBar(total, '\x2592', color, message, _consoleOperator, _locationLine)
            {
                AlternateGetBackToRow = GetStartLocation(),

                // Behavior = LoadingBarBehavior.ClearWhenHundredPercentIsHit
                Behavior = _behavior
            };
            AddLoadingBar(loadingBar);

            if (!_consoleOperator.IsConsoleApp)
                return loadingBar;

            int startLocation = GetStartLocation();
            if (startLocation >= Console.BufferHeight - (_loadingBars.Count * 2) - 1)
            {
                foreach (SingleLoadingBar singleLoadingBar in _loadingBars)
                {
                    if (singleLoadingBar == null)
                        throw new NullReferenceException("singleLoadingBar in CreateNewLoadingBar()");

                    int a = singleLoadingBar.AlternateGetBackToRow;
                    int b = GetStartLocation();
                    if (a != b)
                    {
                        singleLoadingBar.AlternateGetBackToRow = b;
                    }
                }
            }

            /* You do NOT need a GotoStart() here.
             * If you want to place one, then you've made an error somewhere
             * GotoStart(); */
            return loadingBar;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void AddLoadingBar(SingleLoadingBar loadingBar)
        {
            if (loadingBar == null)
                return;

            loadingBar.Update(0);
            _loadingBars.Add(loadingBar);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void CleanUp()
        {
            if (!_consoleOperator.IsConsoleApp)
                return;

            GotoStart();
            Console.CursorTop++;

            lock (_syncRegulate)
            {
                bool goingLast = false;

                for (int i = 0; i < _loadingBars.Count * 2; i++)
                {
                    _consoleOperator.ClearLine(ref _regulate, goingLast);

                    if (i == (_loadingBars.Count * 2) - 2)
                        goingLast = true;
                }
            }

            GotoStart();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public int GetStartLocation()
        {
            return _locationRow - _regulate;
        }


        private void GotoLineAboveNextBar()
        {
            if (!_consoleOperator.IsConsoleApp)
                return;

            int row = GetStartLocation() + (_loadingBars.Count * 2);
            if (row >= Console.BufferHeight)
                throw new Exception("Need more space!");

            Console.CursorTop = row;
        }

        private void MakeSureWeHaveSpaceForNewLoadingBar()
        {
            if (!_consoleOperator.IsConsoleApp)
                return;

            int asd = GetStartLocation() + ((_loadingBars.Count + 1) * 2);
            // ReSharper disable once UnusedVariable
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
                    if (loadingBar == null)
                        throw new NullReferenceException("loadingBar in MakeSureWeHaveSpaceForNewLoadingBar()");

                    loadingBar.UpdateRegular(total);
                }
            }

            GotoStart();
        }


        private void GotoStart()
        {
            Console.CursorTop = GetStartLocation();
            Console.CursorLeft = _locationLine;
        }
    }
}
