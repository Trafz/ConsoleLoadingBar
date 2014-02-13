using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConsoleLoadingBar.Enums;

namespace ConsoleLoadingBar
{
    public class MultipleConsoleLoadingBars : IDisposable
    {
        public MultipleConsoleLoadingBars()
        {
            ResetColors();
            LocationRow = Console.CursorTop;
            LocationLine = Console.CursorLeft;
        }

        public readonly int LocationRow;
        public readonly int LocationLine;

        private readonly HashSet<SingleLoadingBar> _loadingBars = new HashSet<SingleLoadingBar>();
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

            Console.CursorTop = LocationRow + (_loadingBars.Count * 2);
            Console.CursorLeft = 0;

            var loadingBar = new SingleLoadingBar(total, '\x2592', color, message, LoadingbarBehaviour.ClearWhenHundredPercentIsHit);

            AddLoadingBar(loadingBar);

            return loadingBar;
        }
        public void AddLoadingBar(SingleLoadingBar loadingBar)
        {
            loadingBar.Update(0);
            _loadingBars.Add(loadingBar);
        }


        public void CleanUp()
        {
            foreach (SingleLoadingBar loadingBar in LoadingBars)
                loadingBar.Clear();

            Console.CursorLeft = LocationLine;
            Console.CursorTop = LocationRow;
        }

        public void Dispose()
        {
            CleanUp();
        }
    }
}
