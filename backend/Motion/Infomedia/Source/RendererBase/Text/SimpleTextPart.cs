// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleTextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities;

    /// <summary>
    /// Text part created by <see cref="SimpleTextFactory"/>.
    /// </summary>
    public class SimpleTextPart : SimplePartBase, ITextPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTextPart"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this string part should blink.
        /// </param>
        internal SimpleTextPart(string text, Font font, bool blink)
            : base(blink)
        {
            this.Text = text;
            this.Font = font;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font { get; private set; }
    }
}