using ConsoleLoadingBar.Core;
using NUnit.Framework;

namespace ConsoleLoadingBar.Tests
{
    [TestFixture]
    public class SingleLoadingBarTests
    {
        [Test]
        public void SimpleIteration()
        {
            const int total = 100;

            using (var bar = new SingleLoadingBar(total))
            {
                for (int i = 0; i < total; i++)
                {
                    bar.Update();
                }
            }
        }
    }
}
