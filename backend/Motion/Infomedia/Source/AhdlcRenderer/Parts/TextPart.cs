// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Parts
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// A string part of a text, used for layout and format handling.
    /// </summary>
    public sealed class TextPart : PartBase, ITextPart
    {
        private string bidiText;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextPart"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The logical font.
        /// </param>
        public TextPart(string text, Font font)
        {
            this.Text = text;
            this.Font = font;
            this.CharSpacing = font == null ? 1 : font.CharSpacing;
        }

        private TextPart()
        {
        }

        private TextPart(string text, TextPart original)
        {
            this.BitmapFont = original.BitmapFont;
            this.Font = original.Font;
            this.CharSpacing = original.CharSpacing;
            this.Text = text;
            this.SetScaling(1.0);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the bi-directionally corrected text.
        /// This will reverse arabic words and parenthesis if needed.
        /// </summary>
        public string BidiText
        {
            get
            {
                if (this.bidiText != null)
                {
                    return this.bidiText;
                }

                foreach (var c in this.Text)
                {
                    if (BidiUtility.IsCharacterArabHebrew(c, BidiUtility.AvailableCharacters.ArabicHebrew))
                    {
                        return this.bidiText = BidiUtility.GetTextBidi(this.Text);
                    }
                }

                return this.bidiText = this.Text;
            }
        }

        /// <summary>
        /// Gets or sets the bitmap font used for measuring and drawing text.
        /// </summary>
        public IFont BitmapFont { get; set; }

        /// <summary>
        /// Gets the char spacing.
        /// </summary>
        public int CharSpacing { get; private set; }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Sets the scaling factor of this item.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the factor is not equal to 1.0; scaling is not supported in this renderer!
        /// </exception>
        public override void SetScaling(double factor)
        {
            if (factor < 1.0)
            {
                throw new ArgumentOutOfRangeException("factor", "Only factor of 1.0 allowed");
            }

            var x = 0;
            var lastCharSpacing = 0;
            foreach (var c in this.BidiText)
            {
                x += lastCharSpacing;
                var bmp = this.BitmapFont.GetCharacter(c);
                x += bmp.Width;
                lastCharSpacing = BidiUtility.IsCharacterArabHebrew(c, BidiUtility.AvailableCharacters.Arabic)
                                      ? 0
                                      : this.CharSpacing;
            }

            this.Width = x;
            this.Height = this.BitmapFont.Height;
            this.Ascent = this.BitmapFont.Height; // TODO: we could have a look at the "Unterlänge"
            this.HorizontalGapAfter = lastCharSpacing;
        }

        /// <summary>
        /// Tries to split the item into two parts at the given offset.
        /// The last possible split point in this item has to be found (meaning where the width of
        /// the returned <see cref="left"/> item is less than or equal to the given <see cref="offset"/>).
        /// If this item can't be split, the method must return false and <see cref="right"/> must be null.
        /// If the first possible split point is past <see cref="offset"/>, this method should split
        /// at that point and return true.
        /// </summary>
        /// <param name="offset">
        /// The offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left item of the split operation. This is never null.
        /// If the item couldn't be split, this return parameter might be the object this method was called on.
        /// </param>
        /// <param name="right">
        /// The right item of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        public override bool Split(int offset, out PartBase left, out PartBase right)
        {
            var x = 0;
            var lastCharSpacing = 0;
            var lastSeparableIndex = -1;
            var shouldSkipLastSeparable = false;
            for (int index = 0; index < this.Text.Length; index++)
            {
                // TODO: should we rather use the BidiText?
                var c = this.Text[index];
                x += lastCharSpacing;
                var bmp = this.BitmapFont.GetCharacter(c);
                x += bmp.Width;

                if (x >= offset && lastSeparableIndex > 0)
                {
                    left =
                        new TextPart(
                            this.Text.Substring(
                                0, shouldSkipLastSeparable ? lastSeparableIndex : lastSeparableIndex + 1),
                            this);

                    right = new TextPart(this.Text.Substring(lastSeparableIndex + 1), this);
                    return true;
                }

                if (char.IsSeparator(c) || char.IsWhiteSpace(c))
                {
                    lastSeparableIndex = index;
                    shouldSkipLastSeparable = char.IsWhiteSpace(c);
                }

                lastCharSpacing = BidiUtility.IsCharacterArabHebrew(c, BidiUtility.AvailableCharacters.Arabic)
                                      ? 0
                                      : this.CharSpacing;
            }

            left = this;
            right = null;
            return false;
        }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new TextPart
                       {
                           Text = this.Text,
                           BitmapFont = this.BitmapFont,
                           CharSpacing = this.CharSpacing,
                           Font = this.Font
                       };
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public override bool Equals(PartBase other)
        {
            var text = other as TextPart;
            return text != null && this.Text == text.Text;
        }
    }
}