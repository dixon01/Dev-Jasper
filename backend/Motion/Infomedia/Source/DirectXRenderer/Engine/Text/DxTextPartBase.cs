// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTextPartBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextPartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;

    /// <summary>
    /// Base class for all Managed DirectX text parts.
    /// </summary>
    public abstract class DxTextPartBase : DxPart, ITextPart
    {
        private static readonly char[] WrapChars = { ' ', '-' };

        /// <summary>
        /// Initializes a new instance of the <see cref="DxTextPartBase"/> class.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="font">
        ///     The font.
        /// </param>
        /// <param name="blink">
        ///     The blink.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected DxTextPartBase(string text, Font font, bool blink, IDxDeviceRenderContext context)
            : base(blink, context)
        {
            this.Text = text;
            this.Font = font;

            if (font == null)
            {
                return;
            }

            this.Color = font.GetColor();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Measures the size of the given text using the current <see cref="Font"/>.
        /// </summary>
        /// <param name="text">
        /// The text to be measured.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected abstract Size MeasureString(string text);

        /// <summary>
        /// Tries to split the <see cref="Text"/> into two parts at the given pixel offset.
        /// </summary>
        /// <param name="offset">
        /// The pixel offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left part of the split operation. This is null if the method returns false.
        /// </param>
        /// <param name="right">
        /// The right part of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        protected bool Split(int offset, out string left, out string right)
        {
            var index = this.Text.IndexOfAny(WrapChars);
            var lastIndex = -1;

            while (index >= 0)
            {
                var c = this.Text[index];
                var end = c == '-' ? index + 1 : index;
                var first = this.Text.Substring(0, end);
                var size = this.MeasureString(first);
                if (size.Width > offset && lastIndex != -1)
                {
                    break;
                }

                lastIndex = index;
                index = this.Text.IndexOfAny(WrapChars, lastIndex + 1);
            }

            if (lastIndex < 0)
            {
                left = null;
                right = null;
                return false;
            }

            var startNext = lastIndex + 1;
            if (this.Text[lastIndex] == '-')
            {
                lastIndex++;
            }

            left = this.Text.Substring(0, lastIndex);
            right = this.Text.Substring(startNext);
            return true;
        }
    }
}