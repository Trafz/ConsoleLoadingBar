using ConsoleLoadingBar.Core;
using NUnit.Framework;

namespace ConsoleLoadingBar.Tests
{
    [TestFixture]
    public class UtilitiesTests
    {
        [Test]
        public void IsConsoleApp()
        {
            Assert.That(LoadingBarUtilities.IsConsoleApp(), Is.False);
        }
    }
}
