// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTimePart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleTimePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using System.Globalization;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Time part created by <see cref="SimpleTextFactory"/>.
    /// </summary>
    public class SimpleTimePart : SimplePartBase, ITextPart
    {
        private readonly string timeFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTimePart"/> class.
        /// </summary>
        /// <param name="timeFormat">
        /// The time format.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        internal SimpleTimePart(string timeFormat, Font font, bool blink)
            : base(blink)
        {
            this.timeFormat = timeFormat;
            this.Font = font;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return TimeProvider.Current.Now.ToString(this.timeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font { get; private set; }
    }
}