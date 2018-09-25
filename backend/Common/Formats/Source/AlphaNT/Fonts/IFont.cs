// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFont.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFont type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Fonts
{
    using System;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;

    /// <summary>
    /// Interface to access characters of a font.
    /// </summary>
    public interface IFont
    {
        /// <summary>
        /// Gets the font name found inside the font.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the number of characters in this font.
        /// </summary>
        int CharacterCount { get; }

        /// <summary>
        /// Gets the total height of this font in pixels.
        /// Each character has the given height.
        /// </summary>
        int Height { get; }

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
        IBitmap GetCharacter(char character);
    }
}