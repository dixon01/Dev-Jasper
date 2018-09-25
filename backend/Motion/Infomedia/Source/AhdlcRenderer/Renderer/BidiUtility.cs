// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BidiUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BidiUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Static helper class for handling bidirectional texts.
    /// </summary>
    public static class BidiUtility
    {
        private static readonly int[] UnicodeFontSet =
            {
                0x0590, 0x05FF,
                0xFB1D, 0xFB4F,
                0x0600, 0x06FF,
                0x0750, 0x077F,
                0xFB50, 0xFDFF,
                0xFE70, 0xFEFF
            };

        private static readonly Dictionary<char, ArabCodeInfo> ArabCode = new Dictionary<char, ArabCodeInfo>();

        private static readonly Dictionary<CombinedKey, ArabCodeInfo> CombinedArabCode =
            new Dictionary<CombinedKey, ArabCodeInfo>();

        static BidiUtility()
        {
            var input = typeof(BidiUtility).Assembly.GetManifestResourceStream(typeof(BidiUtility), "arabCode.txt");
            if (input != null)
            {
                LoadArabCodeTable(input);
            }
        }

        /// <summary>
        /// The available characters.
        /// </summary>
        [Flags]
        public enum AvailableCharacters
        {
            /// <summary>
            /// Test if the given character is arab
            /// </summary>
            Arabic = 1,

            /// <summary>
            /// Test if the given character is hebrew
            /// </summary>
            Hebrew = 2,

            /// <summary>
            /// Test if the given character is arab or hebrew
            /// </summary>
            ArabicHebrew = 3
        }

        /// <summary>
        /// Checks if the requested character is from the asked type.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// True if the character was from the given range.
        /// </returns>
        public static bool IsCharacterArabHebrew(char character, AvailableCharacters type)
        {
            int start;
            int end;

            switch (type)
            {
                case AvailableCharacters.Arabic:
                    start = 2;
                    end = 6;
                    break;
                case AvailableCharacters.Hebrew:
                    start = 0;
                    end = 2;
                    break;
                case AvailableCharacters.ArabicHebrew:
                    start = 0;
                    end = 6;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            for (var j = start; j < end; j++)
            {
                if (!(character < UnicodeFontSet[j * 2]) || character > UnicodeFontSet[(j * 2) + 1])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the reverse arabic and hebrew text
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The reversed <see cref="string"/>.
        /// </returns>
        public static string GetTextBidi(string text)
        {
            var workText = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                workText.Append(ChangeParentheses(c));
            }

            var len = text.Length - 1;
            var endPos = 0;
            var character = workText[0];
            var reverseText = new StringBuilder();

            // if there are ASCII characters or numbers or blank or '-' leave them there
            for (var i = 0; i < workText.Length; i++)
            {
                if (IsAsciiCharacter(character) || char.IsNumber(character) || character == ' ' || character == '-')
                {
                    reverseText.Append(character);
                    if (endPos < (workText.Length - 1))
                    {
                        character = workText[++endPos];
                    }
                }
            }

            if (endPos != 0)
            {
                if (workText[endPos - 1] == ' ' || character == '-')
                {
                    // Blank character let text or number with blank in first position
                    while (char.IsNumber(character) && (endPos >= workText.Length - 1))
                    {
                        character = workText[++endPos];
                    }
                }
            }
            else
            {
                while (char.IsNumber(character))
                {
                    character = workText[++endPos];
                }

                if (endPos > 0 && (workText[endPos] == ' ' || character == '-'))
                {
                    // Blank let text or number with blank in first position
                    for (var pos1 = 0; pos1 < endPos; pos1++)
                    {
                        character = workText[pos1];
                        reverseText.Append(character);
                    }

                    character = workText[endPos];
                    while (IsAsciiCharacter(character) || char.IsNumber(character) || character == ' '
                           || character == '-')
                    {
                        reverseText.Append(character);
                        character = workText[++endPos];
                    }
                }
                else
                {
                    endPos = 0;
                }
            }

            AppendReverseText(len, endPos, workText.ToString(), reverseText);
            ConvertArabText(reverseText);
            return reverseText.ToString();
        }

        private static bool IsAsciiCharacter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private static char ChangeParentheses(char character)
        {
            switch (character)
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
                    return character;
            }
        }

        private static void AppendReverseText(
            int len,
            int endPos,
            string workText,
            StringBuilder reverseText)
        {
            while (len >= endPos)
            {
                var pos2 = len;
                while ((IsAsciiCharacter(workText[pos2]) || char.IsNumber(workText[pos2])) && pos2 > endPos)
                {
                    pos2--;
                    while ((char.IsNumber(workText[pos2]) || (workText[pos2] == '-') || (workText[pos2] == ' '))
                           && pos2 > endPos)
                    {
                        pos2--;
                    }
                }

                if (pos2 != len)
                {
                    var pos1 = len;
                    len = ++pos2;
                    while (pos2 <= pos1)
                    {
                        reverseText.Append(workText[pos2++]);
                    }
                }
                else
                {
                    pos2 = len;
                    while (pos2 >= endPos && pos2 > 0
                           && (char.IsNumber(workText[pos2]) || (pos2 != len && workText[pos2] == '-')))
                    {
                        pos2--;
                    }

                    if (pos2 < len)
                    {
                        // more numbers, do not reverse
                        var pos1 = len;
                        len = ++pos2;
                        while (pos2 <= pos1)
                        {
                            reverseText.Append(workText[pos2++]);
                        }
                    }
                    else
                    {
                        pos2 = len;
                        reverseText.Append(workText[pos2]);
                    }
                }

                len--;
            }
        }

        private static void LoadArabCodeTable(Stream input)
        {
            using (var reader = new StreamReader(input, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Trim().Split(' ');
                    if (parts.Length < 3)
                    {
                        continue;
                    }

                    int baseCode;
                    if (!ParserUtil.TryParse(parts[0], NumberStyles.HexNumber, null, out baseCode))
                    {
                        continue;
                    }

                    var contexts = new char[parts.Length - 2];
                    for (int i = 2; i < parts.Length; i++)
                    {
                        int ctxCode;
                        if (!ParserUtil.TryParse(parts[i], NumberStyles.HexNumber, null, out ctxCode))
                        {
                            continue;
                        }

                        contexts[i - 2] = (char)ctxCode;
                    }

                    if (parts[1] == "X")
                    {
                        if (contexts.Length < 3)
                        {
                            continue;
                        }

                        // special handling of "X" rows:
                        // [char1] X [char2] [isolated] [final]
                        CombinedArabCode[new CombinedKey((char)baseCode, contexts[0])]
                            = new ArabCodeInfo(contexts[1], contexts[2]);
                        continue;
                    }

                    ArabCode[(char)baseCode] = new ArabCodeInfo(contexts);
                }
            }
        }

        private static void ConvertArabText(StringBuilder reverseText)
        {
            var wasArab = false;
            for (int i = reverseText.Length - 1; i >= 0; i--)
            {
                var c = reverseText[i];
                ArabCodeInfo info;
                if (!ArabCode.TryGetValue(c, out info))
                {
                    wasArab = false;
                    continue;
                }

                if (i > 0)
                {
                    ArabCodeInfo combined;
                    if (CombinedArabCode.TryGetValue(new CombinedKey(c, reverseText[i - 1]), out combined))
                    {
                        // there exists a combination of this and the next character,
                        // let's replace those two with the combined one
                        info = combined;
                        reverseText.Remove(i, 1);
                        i--;
                    }
                }

                ArabCodeInfo nextInfo;
                var nextArab = i > 0 && ArabCode.TryGetValue(reverseText[i - 1], out nextInfo)
                               && (nextInfo.Medial != 0 || nextInfo.Final != 0);
                if (wasArab && nextArab && info.Medial != 0)
                {
                    reverseText[i] = info.Medial;
                }
                else if (wasArab && info.Final != 0)
                {
                    reverseText[i] = info.Final;
                    wasArab = false;
                }
                else if (nextArab && info.Initial != 0)
                {
                    reverseText[i] = info.Initial;
                    wasArab = true;
                }
                else
                {
                    reverseText[i] = info.Isolated;
                    wasArab = false;
                }
            }
        }

        private class ArabCodeInfo
        {
            public readonly char Isolated;

            public readonly char Initial;

            public readonly char Medial;

            public readonly char Final;

            public ArabCodeInfo(char[] contexts)
            {
                this.Isolated = contexts[0];
                if (contexts.Length == 1)
                {
                    return;
                }

                this.Final = contexts[1];
                if (contexts.Length == 2)
                {
                    return;
                }

                this.Initial = contexts[2];
                if (contexts.Length == 3)
                {
                    return;
                }

                this.Medial = contexts[3];
            }

            public ArabCodeInfo(char isolated, char final)
            {
                this.Isolated = isolated;
                this.Final = final;
            }
        }

        private class CombinedKey
        {
            public readonly char First;

            public readonly char Second;

            public CombinedKey(char first, char second)
            {
                this.First = first;
                this.Second = second;
            }

            public override bool Equals(object obj)
            {
                var other = obj as CombinedKey;
                return other != null && other.First == this.First && other.Second == this.Second;
            }

            public override int GetHashCode()
            {
                return this.First.GetHashCode() ^ this.Second.GetHashCode();
            }
        }
    }
}
