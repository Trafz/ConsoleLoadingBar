using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace ConsoleLoadingBar.TestApp
{
    internal class Program
    {
        private const string PressAnyKey = "Press any key to continue . . .";
        private static int _millisecondsTimeout;
        private static bool _hasJustPaused;


        public static void TestCase1()
        {
            StartTestCase();

            TestSingleLoadingBar();
            TestMultipleLoadingBar();
            TestSingleLoadingBar();

            EndTestCase();
        }

        public static void TestCase2()
        {
            StartTestCase();

            for (int i = 0; i <= Console.BufferHeight; i++)
                Console.WriteLine(i);

            TestCase1();

            EndTestCase();
        }

        public static void TestSingleLoadingBar()
        {
            Console.Write("Testing my SingleLoadingBar: ");
            RunSingleLoadingBar();
            Console.WriteLine("Done");
        }

        public static void TestMultipleLoadingBar()
        {
            Console.Write("Testing my MultipleLoadingBar: ");
            RunMultipleLoadingBar();
            Console.WriteLine("Done");
        }


        private static void RunMultipleLoadingBar()
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
                            Thread.Sleep(_millisecondsTimeout * multipleBars.LoadingBars.Count);
                            hasWaited = true;
                        }

                        guess = random.Next(0, amountOfBars);
                    }

                    SingleLoadingBar bar = multipleBars.LoadingBars[guess];
                    if (bar == null)
                        throw new NullReferenceException("bar in RunMultipleLoadingBar()");

                    bar.Update();
                    raceBarrier[guess]++;

                    if (!hasWaited)
                        Thread.Sleep(_millisecondsTimeout * multipleBars.LoadingBars.Count);
                }
            }
        }

        private static void RunSingleLoadingBar()
        {
            using (var loadingBar = new SingleLoadingBar(1000))
            {
                for (int i = 0; i < loadingBar.Total; i++)
                {
                    if (i == loadingBar.Total / 100 * 10)
                        Console.Write("10%");
                    if (i == loadingBar.Total / 100 * 20)
                    {
                        Console.Write("20%");
                        Console.WriteLine(Environment.NewLine + "1/5");
                    }

                    if (i == loadingBar.Total / 100 * 30)
                        Console.Write("30%");
                    if (i == loadingBar.Total / 100 * 40)
                        Console.Write("40%");
                    if (i == loadingBar.Total / 100 * 50)
                        Console.Write("50%");
                    if (i == loadingBar.Total / 100 * 60)
                    {
                        Console.Write("60%");
                        Console.WriteLine(Environment.NewLine + "6/10");
                    }

                    if (i == loadingBar.Total / 100 * 70)
                        Console.Write("70%");
                    if (i == loadingBar.Total / 100 * 80)
                        Console.Write("80%");
                    if (i == loadingBar.Total / 100 * 90)
                        Console.Write("90%");
                    if (i == loadingBar.Total / 100 * 100)
                        Console.Write("100%");

                    loadingBar.Update();
                    Thread.Sleep(_millisecondsTimeout);
                }
            }
        }

        private static void StartTestCase()
        {
            _hasJustPaused = false;
        }

        private static void EndTestCase()
        {
            PauseConsole(string.Format("TestCase completed. {0}", PressAnyKey));
        }

        private static void PauseConsole(string message = PressAnyKey, bool skipIfJustPaused = true)
        {
            if (!Debugger.IsAttached)
                return;

            if (skipIfJustPaused && _hasJustPaused)
                return;

            if (string.IsNullOrWhiteSpace(message) == false)
                Console.Write(message);

            Console.ReadKey();
            _hasJustPaused = true;
        }

        private static void PleaseBeConsistent()
        {
            GetAppSettings();
        }

        private static void KeepCommandOpen()
        {
            if (!Debugger.IsAttached)
                return;

            PauseConsole();
        }

        private static void GetAppSettings()
        {
            string sleepLength = ConfigurationManager.AppSettings["TestApp.SleepLength"];
            if (!int.TryParse(sleepLength, out _millisecondsTimeout))
                _millisecondsTimeout = 2;
        }


        private static void Main()
        {
            PleaseBeConsistent();

            TestCase1();
            TestCase2();

            KeepCommandOpen();
        }
    }
}
