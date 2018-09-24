// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HengartnerTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HengartnerTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Special byte array to string transformer for Hengartner CU5 in Abu Dhabi
    /// which sends unicode inside 8-bit telegrams with a complicated encoding.
    /// </summary>
    public class HengartnerTransformer : Transformer<byte[], string, TransformationConfig>
    {
        private static readonly InvertedRtlUnicodeEncoding SpecialUnicodeEncoding = new InvertedRtlUnicodeEncoding();

        /// <summary>
        /// Transforms the byte array to a string using unicode decoding.
        /// </summary>
        /// <param name="data">the byte array.</param>
        /// <returns>the decoded Hengartner string.</returns>
        protected override string DoTransform(byte[] data)
        {
            if (data.Length == 0)
            {
                return string.Empty;
            }

            if (data[0] != 0)
            {
                return Encoding.ASCII.GetString(data, 0, data.Length);
            }

            if (data.Length == 3 && data[0] == 0 && data[1] == '9' && data[2] == '9')
            {
                // special case for "end of list" record 99
                data[0] = 3;
                return Encoding.ASCII.GetString(data, 0, data.Length);
            }

            var encoding = Encoding.BigEndianUnicode;
            int start = 0;
            bool bigEndian = true;
            var builder = new StringBuilder(data.Length / 2);
            for (int i = 0; i < data.Length; i += 2)
            {
                byte first = data[i];
                byte second = data[i + 1];
                if (bigEndian && first == 0 && second == 4)
                {
                    // after the <04> comes a length indication, we don't care
                    // about it, but we have to ignore it
                    builder.Append(encoding.GetString(data, start, i + 2 - start));
                    i++; // ignore the (single!) length byte
                    start = i + 2;
                }
                else if (first == 0 && second == 10)
                {
                    // newline is always sent in big-endian format
                    builder.Append(encoding.GetString(data, start, i - start));
                    builder.Append('\n');
                    start = i + 2;

                    // after the newline we are for sure in little endian
                    bigEndian = false;
                    encoding = SpecialUnicodeEncoding;
                }
                else if (first == 0 && second == 5)
                {
                    // <05> is always sent in big-endian format
                    builder.Append(encoding.GetString(data, start, i - start));
                    builder.Append('\x05');

                    if (!bigEndian)
                    {
                        // <05> is the last byte we support, therefore we set the
                        // start to the end of the string and break the for loop
                        start = data.Length;
                        break;
                    }

                    start = i + 2;

                    // after the <05> we are for sure in little endian
                    bigEndian = false;
                    encoding = SpecialUnicodeEncoding;
                }
            }

            builder.Append(encoding.GetString(data, start, data.Length - start));

            return builder.ToString();
        }

        /// <summary>
        /// Encoding class that inverts right-to-left strings coming from
        /// CU5 in little-endian Unicode to correct .NET Unicode.
        /// The underlying algorithm is by far not perfect but seems to work
        /// for the current bus lines in Abu Dhabi.
        /// </summary>
        private class InvertedRtlUnicodeEncoding : UnicodeEncoding
        {
            private readonly Dictionary<char, char> arabCode = new Dictionary<char, char>();

            public InvertedRtlUnicodeEncoding()
            {
                this.LoadArabCode();
            }

            [SuppressMessage(
                "StyleCopPlus.StyleCopPlusRules",
                "SP2101:MethodMustNotContainMoreLinesThan",
                Justification = "Can't be simplified without making the code more complex")]
            public override string GetString(byte[] bytes, int index, int count)
            {
                // trim away spaces at the beginning
                while (count > 0 && bytes[index] == ' ' && bytes[index + 1] == 0)
                {
                    count -= 2;
                    index += 2;
                }

                if (count == 0)
                {
                    return string.Empty;
                }

                var chars = this.GetChars(bytes, index, count);
                var inverted = new char[chars.Length];
                int outOff = 0;
                int lastSpace = inverted.Length;
                bool hadUninverted = chars[0] <= 0xFF;

                for (int i = chars.Length - 1; i >= -1; i--)
                {
                    // we go until -1 and then just "insert" a space at the end
                    // to force a final flush of uninverted characters if necessary
                    char c = i < 0 ? ' ' : this.ToBaseForm(chars[i]);
                    bool isArabic = c > 0xFF;
                    if (!isArabic && !hadUninverted && (c == '(' || c == ')'))
                    {
                        // this could be a parenthesis around Arabic text
                        if (i > 0)
                        {
                            // is the next character to be inverted?
                            isArabic = chars[i - 1] > 0xFF;
                        }
                        else
                        {
                            // did we have inverted characters before?
                            isArabic = lastSpace > i + 1;
                        }
                    }

                    if (!hadUninverted && isArabic)
                    {
                        inverted[outOff++] = c;
                    }
                    else if (c == ' ')
                    {
                        if (hadUninverted)
                        {
                            for (int j = i + 1; j < lastSpace; j++)
                            {
                                inverted[outOff++] = InvertParentheses(chars[j]);
                            }
                        }

                        if (i < 0)
                        {
                            break;
                        }

                        lastSpace = i;
                        hadUninverted = false;
                        inverted[outOff++] = c;
                    }
                    else
                    {
                        hadUninverted = true;
                    }
                }

                // weird case: if the string ends in an ASCII character (or is it only for parentheses???),
                // the first word is not inverted correctly, so we have to
                // invert it ourselves (not sure what happens if this is a space!)
                if (inverted[inverted.Length - 1] <= 0xFF)
                {
                    int pos = System.Array.IndexOf(inverted, ' ');
                    if (pos > 0)
                    {
                        System.Array.Reverse(inverted, 0, pos);
                    }
                }

                var str = new string(inverted);
                return str;
            }

            private static char InvertParentheses(char c)
            {
                switch (c)
                {
                    case '(':
                        return ')';
                    case ')':
                        return '(';
                    case '[':
                        return ']';
                    case ']':
                        return '[';
                    case '{':
                        return '}';
                    case '}':
                        return '{';
                    default:
                        return c;
                }
            }

            private void LoadArabCode()
            {
                var asm = Assembly.GetExecutingAssembly();
                var stream = asm.GetManifestResourceStream(this.GetType().Namespace + ".arabCode.txt");
                if (stream == null)
                {
                    return;
                }

                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(' ');
                        if (parts.Length < 3)
                        {
                            continue;
                        }

                        if (parts[1] == "X")
                        {
                            // we ignore "X" rows since they
                            // define ligatures which we can't handle correctly;
                            // this isn't a problem since those characters will be
                            // displayed correctly even if they are in contextual form
                            continue;
                        }

                        int baseCode;
                        if (!ParserUtil.TryParse(parts[0], NumberStyles.HexNumber, null, out baseCode))
                        {
                            continue;
                        }

                        for (int i = 2; i < parts.Length; i++)
                        {
                            int ctxCode;
                            if (ParserUtil.TryParse(parts[i], NumberStyles.HexNumber, null, out ctxCode))
                            {
                                // there are some rare duplicates, but they are only "self-mappings",
                                // meaning that the characters are mapped to their own value, that's fine
                                this.arabCode[(char)ctxCode] = (char)baseCode;
                            }
                        }
                    }
                }
            }

            private char ToBaseForm(char c)
            {
                char output;
                return this.arabCode.TryGetValue(c, out output) ? output : c;
            }
        }
    }
}