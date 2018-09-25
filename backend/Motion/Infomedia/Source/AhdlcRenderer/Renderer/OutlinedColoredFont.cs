// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutlinedColoredFont.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutlinedColoredFont type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// An <see cref="IFont"/> implementation that provides characters
    /// with a given color and an outline (of 1 pixel).
    /// </summary>
    public class OutlinedColoredFont : IFont
    {
        private readonly IFont font;

        private readonly PixelColor outlineColor;

        private readonly Dictionary<char, IBitmap> outlinedBitmaps;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlinedColoredFont"/> class.
        /// </summary>
        /// <param name="font">
        /// The base (monochrome) font.
        /// </param>
        /// <param name="color">
        /// The color in which to provide the characters.
        /// </param>
        /// <param name="outlineColor">
        /// The color in which to draw a one-pixel border around every character.
        /// </param>
        public OutlinedColoredFont(IFont font, Color color, Color outlineColor)
        {
            this.font = new ColoredFont(font, color);
            this.outlineColor = new PixelColor(outlineColor.R, outlineColor.G, outlineColor.B);
            this.outlinedBitmaps = new Dictionary<char, IBitmap>(font.CharacterCount);
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
                // two pixels more for the outline (one on top, one on the bottom)
                return this.font.Height + 2;
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
            IBitmap bitmap;
            if (this.outlinedBitmaps.TryGetValue(character, out bitmap))
            {
                return bitmap;
            }

            var origBitmap = this.font.GetCharacter(character);
            var newBitmap = this.CreateOutlinedBitmap(origBitmap);

            this.outlinedBitmaps.Add(character, newBitmap);
            return newBitmap;
        }

        private IBitmap CreateOutlinedBitmap(IBitmap origBitmap)
        {
            var newBitmap = new SimpleBitmap(origBitmap.Width + 2, origBitmap.Height + 2);

            for (int x = 0; x < newBitmap.Width; x++)
            {
                var posX = x - 1;

                for (int y = 0; y < newBitmap.Height; y++)
                {
                    var posY = y - 1;

                    if (posX < 0 || posY < 0 || posX >= origBitmap.Width || posY >= origBitmap.Height)
                    {
                        this.DrawOutlinePixel(newBitmap, origBitmap, posX, posY, 1, 1);
                        continue;
                    }

                    var pixel = origBitmap.GetPixel(posX, posY);
                    if (pixel.Transparent)
                    {
                        this.DrawOutlinePixel(newBitmap, origBitmap, posX, posY, 1, 1);
                        continue;
                    }

                    newBitmap.SetPixel(x, y, pixel);
                }
            }

            return newBitmap;
        }

        private void DrawOutlinePixel(
            SimpleBitmap newBitmap, IBitmap origBitmap, int x, int y, int deltaX, int deltaY)
        {
            var width = origBitmap.Width;
            var height = origBitmap.Height;
            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = y - 1; j <= y + 1; j++)
                {
                    if ((i >= 0 && j >= 0 && i < width && j < height)
                        && (i != x || j != y)
                        && !origBitmap.GetPixel(i, j).Transparent)
                    {
                        newBitmap.SetPixel(
                            x + deltaX, y + deltaY, this.outlineColor.R, this.outlineColor.G, this.outlineColor.B);
                        return;
                    }
                }
            }
        }
    }
}