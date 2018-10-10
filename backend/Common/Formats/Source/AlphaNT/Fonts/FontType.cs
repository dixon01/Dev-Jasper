// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Fonts
{
    /// <summary>
    /// The font type for a given font.
    /// </summary>
    public enum FontType
    {
        /// <summary>
        /// Type is Fnt Font.
        /// </summary>
        FntFont = 1,

        /// <summary>
        /// Type is Fon Font.
        /// </summary>
        FonFont = 2,

        /// <summary>
        /// Type is arab Unicode.
        /// </summary>
        FonUnicodeArab = 'A',

        /// <summary>
        /// Type is hebrew Unicode.
        /// </summary>
        FonUnicodeHebrew = 'H',

        /// <summary>
        /// Type is chines Unicode.
        /// </summary>
        FonUnicodeChines = 'C',

        /// <summary>
        /// Type is Cux Font.
        /// </summary>
        CUxFont = 'X',
    }
}
