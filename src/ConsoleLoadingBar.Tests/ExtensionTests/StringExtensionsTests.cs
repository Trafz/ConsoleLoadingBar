using NUnit.Framework;

namespace ConsoleLoadingBar.Tests.ExtensionTests
{
    using ConsoleLoadingBar.Extensions;

    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("1234567890", 20, ExpectedResult = "1234567890")]
        [TestCase("1234567890", 10, ExpectedResult = "1234567890")]
        [TestCase("1234567890", 9, ExpectedResult = "123456...")]
        [TestCase("1234567890", 7, ExpectedResult = "1234...")]
        [TestCase("1234567890", 6, ExpectedResult = "123...")]
        [TestCase("1234567890", 5, ExpectedResult = "#####")]
        [TestCase("1234567890", 1, ExpectedResult = "#")]
        [TestCase("1234567890", 0, ExpectedResult = "")]
        [TestCase("1234567890", -10, ExpectedResult = "")]
        [TestCase("", 10, ExpectedResult = "")]
        [TestCase("", 0, ExpectedResult = "")]
        [TestCase("", -10, ExpectedResult = "")]
        [TestCase(null, 10, ExpectedResult = "")]
        [TestCase(null, 0, ExpectedResult = "")]
        [TestCase(null, -10, ExpectedResult = "")]
        public string ShortenReturnCorrectResult(string input, int maxLength)
        {
            return input.Shorten(maxLength);
        }
    }
}
