// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Fonts
{
    using System;
    using System.IO;
    using System.Text;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Font file class for reading FON and FNT files.
    /// </summary>
    public class FontFile : IFont
    {
        private const char FirstCharacterBlank = (char)0x0020;
        private const char LastCharacterArabic = (char)0xFFFF;
        private const char LastCharacterHebrew = (char)0x05E0;

        private readonly IByteAccess fontData;

        private readonly int bytesPerCharacterRow;

        private char lastCharacter;

        private FontType fontType;

        private char firstCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFile"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename from which to read the font.
        /// </param>
        public FontFile(string filename)
            : this(File.OpenRead(filename), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFile"/> class.
        /// </summary>
        /// <param name="input">
        /// The input stream from which to read the font.
        /// </param>
        /// <param name="closeStream">
        /// Flag indicating whether the <see cref="input"/> should be closed
        /// before this method returns or throws an exception.
        /// </param>
        public FontFile(Stream input, bool closeStream)
            : this(new BinaryFileReader(input, closeStream), closeStream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFile"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader from which to read the font.
        /// </param>
        /// <param name="closeReader">
        /// Flag indicating whether the <see cref="reader"/> should be closed
        /// before this method returns or throws an exception.
        /// </param>
        internal FontFile(BinaryFileReader reader, bool closeReader)
        {
            try
            {
                this.CharacterCount = (reader.ReadByte() & 0x80) == 0 ? 96 : 224; // 0
                this.bytesPerCharacterRow = reader.ReadByte(); // 1
                if (this.bytesPerCharacterRow <= 0 || this.bytesPerCharacterRow >= 64)
                {
                    // just some arbitrary limit
                    throw new FileFormatException("Unsupported bytes per character: " + this.bytesPerCharacterRow);
                }

                this.Height = reader.ReadByte(); // 2
                this.Name = Encoding.ASCII.GetString(reader.ReadBytes(8).ToArray(), 0, 8); // 3..10
                var endMarker = reader.ReadByte(); // 11
                switch (endMarker)
                {
                    case 0x43:
                        var identifier = reader.ReadLsbUInt16();  // 13 and 14
                        switch (identifier)
                        {
                            case 0x5801:
                                this.CharacterCount = reader.ReadLsbUInt16(); // 15 and 16 CUx
                                this.SetFontTypeParameter((char)0, (char)0, FontType.CUxFont);
                                break;

                            default:
                                this.SetFontTypeParameter(
                                    (char)identifier, (char)reader.ReadLsbUInt16(), FontType.FonUnicodeChines);
                                this.CharacterCount = this.lastCharacter - this.firstCharacter; // 15 and 16 Chines
                                break;
                        }

                        break;

                    case 0x41:  // 'A' endMarker in font file for arabic font
                    case 0x48:  // 'H' endMarker in font file for hebrew font
                        if (endMarker == 0x41)
                        {
                            this.SetFontTypeParameter(
                                FirstCharacterBlank, LastCharacterArabic, FontType.FonUnicodeArab);
                        }
                        else
                        {
                            this.SetFontTypeParameter(
                                FirstCharacterBlank, LastCharacterHebrew, FontType.FonUnicodeHebrew);
                        }

                        this.CharacterCount = (reader.Length - 12) /
                            (1 + (this.Height * this.bytesPerCharacterRow));
                        break;

                    default:
                        if (endMarker != 0 && endMarker != 0xFF && endMarker != 0xFE)
                        {
                            throw new FileFormatException(
                                "Unknown end of header: 0x" + endMarker.ToString("X2"));
                        }

                        this.SetFntFonParameter();

                        break;
                }

                var offset = this.GetOffset();
                this.fontData = reader.ReadBytes(
                    ((this.bytesPerCharacterRow * this.Height) + offset) * this.CharacterCount);
            }
            finally
            {
                if (closeReader)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Gets the font name found inside the font.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the number of characters in this font.
        /// </summary>
        public int CharacterCount { get; private set; }

        /// <summary>
        /// Gets the total height of this font in pixels.
        /// Each character has the given height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets a single character from this font.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// The <see cref="IBitmap"/> representing the character.
        /// The pixels of the character will be white, everything else transparent.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// if the character is not defined in this font.
        /// </exception>
        public IBitmap GetCharacter(char character)
        {
            int characterSize;
            char mappedCharacter = (char)0;

            var cuxOffset = 0;
            if (this.fontType == FontType.CUxFont)
            {
                characterSize = (this.bytesPerCharacterRow * this.Height) + 3;
                cuxOffset = 2;
                if (this.FindCUxCharacter(character, characterSize, ref mappedCharacter))
                {
                    character = mappedCharacter;
                }
            }
            else
            {
                characterSize = (this.bytesPerCharacterRow * this.Height) + 1;
                mappedCharacter = character;
            }

            var charOffset = (character - this.firstCharacter) * characterSize;
            charOffset = charOffset + cuxOffset;

            if (!this.CheckIfCharacterIsValid(character, this.fontType) || mappedCharacter != character)
            {
                throw new ArgumentOutOfRangeException("character", "Font doesn't have character " + character);
            }

            return new CharacterBitmap(this.fontData, charOffset, this.bytesPerCharacterRow, this.Height);
        }

        private void SetFntFonParameter()
        {
            if (this.CharacterCount == 96)
            {
                this.SetFontTypeParameter(FirstCharacterBlank, (char)0x07F, FontType.FntFont);
            }
            else
            {
                this.SetFontTypeParameter(FirstCharacterBlank, (char)0x0FF, FontType.FonFont);
            }
        }

        private bool CheckIfCharacterIsValid(char character, FontType font)
        {
            switch (font)
            {
                case FontType.FntFont:
                case FontType.FonFont:
                case FontType.FonUnicodeArab:
                case FontType.FonUnicodeHebrew:
                case FontType.FonUnicodeChines:
                    if (character >= this.firstCharacter && character - this.firstCharacter < this.CharacterCount)
                        {
                            return true;
                        }

                    break;

                case FontType.CUxFont:
                    return true;
           }

            return false;
        }

        private bool FindCUxCharacter(char character, int characterSize, ref char mappedcharacter)
        {
            if (this.fontData == null)
            {
                return false;
            }

            var numberOfCharacters = this.fontData.Length / characterSize;
            var characterPosition = 0;
            for (var i = 0; i < numberOfCharacters; i++)
            {
                int unicode = this.fontData[characterPosition];
                characterPosition++;
                unicode += this.fontData[characterPosition] * 256;
                if (unicode == character)
                {
                    mappedcharacter = (char)i;
                    return true;
                }

                characterPosition += characterSize - 1;
            }

            return false;
        }

        private void SetFontTypeParameter(char startCharacter, char endCharacter, FontType type)
        {
            this.firstCharacter = startCharacter;
            this.lastCharacter = endCharacter;
            this.fontType = type;
        }

        private int GetOffset()
        {
            return this.fontType != FontType.CUxFont ? 1 : 3;
        }

        private class CharacterBitmap : BitmapBase
        {
            private readonly IByteAccess data;

            private readonly int bytesPerCharacterRow;

            private readonly int offset;

            public CharacterBitmap(IByteAccess data, int offset, int bytesPerCharacterRow, int height)
            {
                this.Width = data[offset] & 0x7F;
                this.Height = height;

                this.data = data;
                this.offset = offset + 1;
                this.bytesPerCharacterRow = bytesPerCharacterRow;
            }

            public override IColor GetPixel(int x, int y)
            {
                if (x < 0 || x >= this.Width || y < 0 || y >= this.Height)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var pos = (y * this.bytesPerCharacterRow) + this.offset + (x / 8);
                return (this.data[pos] & (1 << (7 - (x % 8)))) == 0 ? Colors.Transparent : Colors.White;
            }
        }
    }
}
