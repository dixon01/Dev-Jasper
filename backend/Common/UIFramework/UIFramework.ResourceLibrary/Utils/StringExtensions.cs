namespace Luminator.UIFramework.ResourceLibrary.Utils
{
    using System;
    using System.Collections.Generic;

    public static class StringExtensions
    {
        public static IEnumerable<string> Splice(this string s, int spliceLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (spliceLength < 1)
                throw new ArgumentOutOfRangeException("spliceLength");

            if (s.Length == 0)
                yield break;
            var start = 0;
            for (var end = spliceLength; end < s.Length; end += spliceLength)
            {
                yield return s.Substring(start, spliceLength);
                start = end;
            }
            yield return s.Substring(start);
        }

        public static string IntToBinaryString(this int number)
        {
            var binary = string.Empty;
            while (number > 0)
            {
                // Logical AND the number and prepend it to the result string
                binary = (number & 1) + binary;
                number = number >> 1;
            }

            return binary.PadLeft(16, '0');
        }
    }
}
