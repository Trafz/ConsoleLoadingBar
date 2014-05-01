using JetBrains.Annotations;

namespace ConsoleLoadingBar.Extensions
{
    public static class StringExtensions
    {
        [Pure, NotNull]
        public static string Shorten([CanBeNull] this string s, int maxLength)
        {
            if (maxLength < 1)
                return string.Empty;
            if (maxLength < 6)
                return new string('#', maxLength);

            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            const string dots = "...";

            int amountOfChars = maxLength - dots.Length;
            if (amountOfChars <= 0 || amountOfChars >= s.Length)
            {
                return s;
            }

            return s.Length <= maxLength ? s : s.Substring(0, amountOfChars) + dots;
        }
    }
}
