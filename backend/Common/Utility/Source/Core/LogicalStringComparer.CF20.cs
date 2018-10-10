// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalStringComparer.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogicalStringComparer type.
//   Copied from Cadru.Core/Collections/LogicalStringComparer.cs
// </summary>
// <license>
//    Licensed under the Microsoft Public License (Ms-PL) (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Compares two strings for equivalence, ignoring case, in natural numeric order.
    /// </summary>
    /// <remarks>
    /// <para>Windows implements natural numeric sorting inside the <c>StrCmpLogicalW</c>.
    /// This function is available on Windows XP or higher.</para>
    /// <para>This implementation is not 100% compatible with <c>StrCmpLogicalW</c>
    /// It gives the same results for the numeric sort, with the exception of strings containing non-alphanumeric ASCII
    /// characters. The code relies on the current locale to find the order of the characters.</para>
    /// <para>The code here will order files that start with special characters based on the code table order.
    /// Windows Explorer uses another order.</para>
    /// <para><example>Windows Explorer: (1.txt, [1.txt, _1.txt, =1.txt</example></para>
    /// <para><example>this code: (1.txt, =1.txt, [1.txt, _1.txt</example></para>
    /// </remarks>
    public partial class LogicalStringComparer : StringComparer
    {
        private readonly CultureInfo cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalStringComparer"/> class using the
        /// <see cref="CultureInfo.CurrentCulture"/> of the current thread.
        /// </summary>
        /// <remarks>When the <see cref="LogicalStringComparer"/> instance is created using
        /// this constructor, the <see cref="CultureInfo.CurrentCulture"/> of the
        /// current thread is saved. Comparison procedures use the saved
        /// culture to determine the sort order and casing rules; therefore,
        /// string comparisons might have different results depending on the
        /// culture. For more information on culture-specific comparisons, see
        /// the <see cref="System.Globalization"/> namespace and
        /// <see href="http://msdn.microsoft.com/en-us/library/vstudio/h6270d0z(v=vs.100).aspx">
        /// Encoding and Localization</see>.
        /// </remarks>
        public LogicalStringComparer()
            : this(CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalStringComparer"/> class using
        /// the specified <see cref="System.Globalization.CultureInfo"/>.
        /// </summary>
        /// <param name="culture">The <see cref="CultureInfo"/>
        /// to use for the new <see cref="LogicalStringComparer"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="culture"/> is a <see langword="null" />.</exception>
        /// <rermarks>Comparison procedures use the specified <see cref="CultureInfo"/> to determine
        /// the sort order and casing rules. String comparisons might have different results
        /// depending on the culture. For more information on culture-specific comparisons, see
        /// the <see cref="System.Globalization"/> namespace and
        /// <see href="http://msdn.microsoft.com/en-us/library/vstudio/h6270d0z(v=vs.100).aspx">
        /// Encoding and Localization</see>.
        /// </rermarks>
        public LogicalStringComparer(CultureInfo culture)
        {
            this.cultureInfo = culture;
        }

        /// <summary>
        /// Performs a case-insensitive comparison of two string objects and returns a value
        /// indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Condition</term>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>, with casing ignored.</description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>, with casing ignored.</description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>, with casing ignored.</description>
        /// </item>
        /// </list></returns>
        public override int Compare(object x, object y)
        {
            return this.Compare(x as string, y as string);
        }

        /// <summary>
        /// Performs a case-insensitive comparison of two strings and returns a value
        /// indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">The first string to compare.</param>
        /// <param name="y">The second string to compare.</param>
        /// <returns>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Condition</term>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>, with casing ignored.</description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>, with casing ignored.</description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>, with casing ignored.</description>
        /// </item>
        /// </list></returns>
        public override int Compare(string x, string y)
        {
            if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(x))
            {
                return -1;
            }

            if (string.IsNullOrEmpty(y))
            {
                return 1;
            }

            bool sp1 = char.IsLetterOrDigit(x[0]);
            bool sp2 = char.IsLetterOrDigit(y[0]);

            if (sp1 && !sp2)
            {
                return 1;
            }

            if (!sp1 && sp2)
            {
                return -1;
            }

            return this.CompareStrings(x, y);
        }

        /// <summary>
        /// Returns a value indicating whether two objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the two objects are strings and their values are equal;
        /// otherwise, <see langword="false"/>. </returns>
        public override bool Equals(object x, object y)
        {
            return this.Equals(x as string, y as string);
        }

        /// <summary>
        /// Returns a value indicating whether two instances of string are equal.
        /// </summary>
        /// <param name="x">The first string to compare.</param>
        /// <param name="y">The second string to compare.</param>
        /// <returns><see langword="true"/> if the two string values are equal;
        /// otherwise, <see langword="false"/>. </returns>
        public override bool Equals(string x, string y)
        {
            return string.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be
        /// returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="ArgumentNullException">The type of <paramref name="obj"/> is a
        /// reference type and <paramref name="obj"/> is a <see langword="null"/>.
        /// </exception>
        public override int GetHashCode(object obj)
        {
            var s1 = obj as string;

            if (s1 == null)
            {
                throw new ArgumentException("Argument must be a string");
            }

            return s1.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for the specified string.
        /// </summary>
        /// <param name="obj">The string for which a hash code is to be
        /// returned.</param>
        /// <returns>A hash code for the specified string.</returns>
        /// <exception cref="ArgumentNullException">The type of <paramref name="obj"/> is a
        /// reference type and <paramref name="obj"/> is a <see langword="null"/>.
        /// </exception>
        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

        private static int CompareNumbers(string s1, int lengthS1, ref int i1, string s2, int lengthS2, ref int i2)
        {
            int nonZeroStart1;
            int nonZeroStart2;
            int end1;
            int end2;

            ScanNumber(s1, lengthS1, i1, out nonZeroStart1, out end1);
            ScanNumber(s2, lengthS2, i2, out nonZeroStart2, out end2);

            int start1 = i1;
            i1 = end1 - 1;
            int start2 = i2;
            i2 = end2 - 1;

            int length1 = end2 - nonZeroStart2;
            int length2 = end1 - nonZeroStart1;

            if (length1 == length2)
            {
                int r;
                for (int j1 = nonZeroStart1, j2 = nonZeroStart2; j1 <= i1; j1++, j2++)
                {
                    r = s1[j1] - s2[j2];
                    if (0 != r)
                    {
                        return r;
                    }
                }

                length1 = end1 - start1;
                length2 = end2 - start2;

                if (length1 == length2)
                {
                    return 0;
                }
            }

            if (length1 > length2)
            {
                return -1;
            }

            return 1;
        }

        private static void ScanNumber(string s, int length, int start, out int nonZeroStart, out int end)
        {
            nonZeroStart = start;
            end = start;

            bool countZeros = true;
            char c = s[end];

            while (true)
            {
                if (countZeros)
                {
                    if ('0' == c)
                    {
                        nonZeroStart++;
                    }
                    else
                    {
                        countZeros = false;
                    }
                }

                end++;

                if (end >= length)
                {
                    break;
                }

                c = s[end];

                if (!char.IsDigit(c))
                {
                    break;
                }
            }
        }

        private int CompareStrings(string x, string y)
        {
            bool sp1;
            bool sp2;
            int lengthOfX = x.Length;
            int lengthOfY = y.Length;

            int i1 = 0, i2 = 0;

            while (true)
            {
                var c1 = x[i1];
                char c2 = y[i2];

                sp1 = char.IsDigit(c1);
                sp2 = char.IsDigit(c2);

                int r;
                if (!sp1 && !sp2)
                {
                    r = this.CompareChars(c1, c2);
                    if (0 != r)
                    {
                        return r;
                    }
                }
                else if (sp1 && sp2)
                {
                    r = CompareNumbers(x, lengthOfX, ref i1, y, lengthOfY, ref i2);
                    if (0 != r)
                    {
                        return r;
                    }
                }
                else if (sp1)
                {
                    return -1;
                }
                else
                {
                    // sp2 == true
                    return 1;
                }

                i1++;
                i2++;

                if (i1 >= lengthOfX)
                {
                    if (i2 >= lengthOfY)
                    {
                        return 0;
                    }

                    return -1;
                }

                if (i2 >= lengthOfY)
                {
                    return 1;
                }
            }
        }

        private int CompareChars(char c1, char c2)
        {
            if (c1 == c2)
            {
                return 0;
            }

            var letter1 = char.IsLetter(c1);
            var letter2 = char.IsLetter(c2);

            if (letter1 && letter2)
            {
                c1 = char.ToUpper(c1, this.cultureInfo);
                c2 = char.ToUpper(c2, this.cultureInfo);

                return c1 - c2;
            }

            if (!letter1 && !letter2)
            {
                return c1 - c2;
            }

            if (letter1)
            {
                return 1;
            }

            return -1;
        }
    }
}
