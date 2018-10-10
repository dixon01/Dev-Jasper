// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredFont.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColoredFont type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using System;
    using System.Drawing;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Fonts;

    /// <summary>
    /// An <see cref="IFont"/> implementation that provides characters with a given color (instead of white).
    /// </summary>
    public class ColoredFont : IFont
    {
        private readonly IFont font;

        private readonly IColor color;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColoredFont"/> class.
        /// </summary>
        /// <param name="font">
        /// The base (monochrome) font.
        /// </param>
        /// <param name="color">
        /// The color in which to provide the characters.
        /// </param>
        public ColoredFont(IFont font, Color color)
        {
            this.font = font;
            this.color = new SimpleColor(color.R, color.G, color.B);
        }

        /// <summary>
        /// Gets the font name found inside the font.
        /// </summary>
        public string Name
        {
            get
            {
                return this.font.Name;
            }
        }

        /// <summary>
        /// Gets the number of characters in this font.
        /// </summary>
        public int CharacterCount
        {
            get
            {
                return this.font.CharacterCount;
            }
        }

        /// <summary>
        /// Gets the total height of this font in pixels.
        /// Each character has the given height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.font.Height;
            }
        }

        /// <summary>
        /// Gets a single character from this font.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// The <see cref="IBitmap"/> representing the character.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// if the character is not defined in this font.
        /// </exception>
        public IBitmap GetCharacter(char character)
        {
            return new ColorCharacterBitmap(this.font.GetCharacter(character), this.color);
        }

        private class ColorCharacterBitmap : IBitmap
        {
            private readonly IBitmap bitmap;

            private readonly IColor color;

            public ColorCharacterBitmap(IBitmap bitmap, IColor color)
            {
                this.bitmap = bitmap;
                this.color = color;
            }

            public int Width
            {
                get
                {
                    return this.bitmap.Width;
                }
            }

            public int Height
            {
                get
                {
                    return this.bitmap.Height;
                }
            }

            public IColor GetPixel(int x, int y)
            {
                var pixel = this.bitmap.GetPixel(x, y);
                return pixel.Transparent ? pixel : this.color;
            }
        }
    }
}