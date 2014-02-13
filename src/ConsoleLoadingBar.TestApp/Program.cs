using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleLoadingBar.TestApp
{
    class Program
    {
        static void Main()
        {
            PleaseBeConsistent();

            TestSingleLoadingBar();
            TestMultipleLoadingBar();
            TestSingleLoadingBar();

            KeepCommandOpen();
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
                    while (raceBarrier[guess] == total)
                        guess = random.Next(0, amountOfBars);

                    multipleBars.LoadingBars[guess].Update();
                    raceBarrier[guess]++;

                    Thread.Sleep(10);
                }
            }
        }

        public static void TestSingleLoadingBar()
        {
            Console.Write("Testing my SingleLoadingBar: ");
            RunSingleLoadingBar();
            Console.WriteLine("Done");
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
                    Thread.Sleep(2);
                }
            }
        }


        private static void PleaseBeConsistent()
        {
            GetAppSettings();
        }
        private static void KeepCommandOpen()
        {
            if (!Debugger.IsAttached)
                return;

            Console.Write("Press any key to continue . . ."); Console.ReadKey();
        }

        private static void GetAppSettings()
        {
        }
    }
}
