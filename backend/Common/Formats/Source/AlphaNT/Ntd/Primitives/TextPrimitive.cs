// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextPrimitive.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextPrimitive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    using System;

    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Primitive that draws a text.
    /// </summary>
    public class TextPrimitive : PositionGraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextPrimitive"/> class.
        /// </summary>
        /// <param name="address">
        /// The address of the text.
        /// </param>
        /// <param name="fontIndex">
        /// The font index.
        /// </param>
        /// <param name="spacing">
        /// The spacing.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="border">
        /// The border style.
        /// </param>
        /// <param name="position">
        /// The position data.
        /// </param>
        /// <param name="displayParam">
        /// The display parameter.
        /// </param>
        /// <param name="offsetX">
        /// The horizontal offset.
        /// </param>
        /// <param name="offsetY">
        /// The vertical offset.
        /// </param>
        internal TextPrimitive(
            IntPtr address,
            int fontIndex,
            int spacing,
            IColor color,
            TextOutline border,
            int position,
            byte displayParam,
            int offsetX,
            int offsetY)
            : base(position, displayParam, offsetX, offsetY)
        {
            this.TextAddress = address;
            this.FontIndex = fontIndex;
            this.CharacterSpacing = spacing;
            this.ForegroundColor = color;
            this.Outline = border;
        }

        /// <summary>
        /// Gets the font index. See also <see cref="NtdFile.GetFont"/>.
        /// </summary>
        public int FontIndex { get; private set; }

        /// <summary>
        /// Gets the character spacing in pixels.
        /// </summary>
        public int CharacterSpacing { get; private set; }

        /// <summary>
        /// Gets the foreground color.
        /// </summary>
        public IColor ForegroundColor { get; private set; }

        /// <summary>
        /// Gets the kind of outline to draw around a text.
        /// </summary>
        public TextOutline Outline { get; private set; }

        /// <summary>
        /// Gets the address of the text in the file. See also <see cref="NtdFile.GetString"/>.
        /// </summary>
        public IntPtr TextAddress { get; private set; }
    }
}