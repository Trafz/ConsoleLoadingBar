using System;
using System.Threading;
using ConsoleLoadingBar.Core;
using NUnit.Framework;

namespace ConsoleLoadingBar.Tests
{
    [TestFixture]
    public class MultipleLoadingBarsTests
    {
        [Test]
        public void SimpleIteration()
        {
            const int total = 100;
            const int amountOfBars = 5;

            using (var multipleBars = new MultipleConsoleLoadingBars())
            {
                for (int i = 0; i < amountOfBars; i++)
                    multipleBars.CreateNewLoadingBar(total);

                var random = new Random();
                var raceBarrier = new int[amountOfBars];

                for (int i = 0; i < total * amountOfBars; i++)
                {
                    int guess = random.Next(0, amountOfBars);
                    bool hasWaited = false;
                    while (raceBarrier[guess] == total)
                    {
                        if (!hasWaited)
                        {
                            Thread.Sleep(multipleBars.LoadingBars.Count);
                            hasWaited = true;
                        }

                        guess = random.Next(0, amountOfBars);
                    }

                    SingleLoadingBar bar = multipleBars.LoadingBars[guess];
                    if (bar == null)
                        throw new NullReferenceException("bar in RunMultipleLoadingBar()");

                    bar.Update();
                    raceBarrier[guess]++;

                    Thread.Sleep(multipleBars.LoadingBars.Count);
                }
            }
        }
    }
}
