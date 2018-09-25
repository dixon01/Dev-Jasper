// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Size.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Size type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Font size BBCode [size=xxx][/size] tag.
    /// </summary>
    public class Size : BbValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent tag.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="sizeText">
        /// The size string.
        /// </param>
        internal Size(BbBranch parent, string tagName, string sizeText)
            : base(parent, tagName, sizeText)
        {
        }

        /// <summary>
        /// Gets the size string defined.
        /// </summary>
        public string SizeText
        {
            get
            {
                return this.Value;
            }
        }

        /// <summary>
        /// Calculates the real size depending on the input height
        /// and the defined size (relative "%" or absolute).
        /// </summary>
        /// <param name="parentHeight">
        /// The parent height.
        /// </param>
        /// <returns>
        /// the calculated height or <see cref="parentHeight"/> if there was
        /// an error.
        /// </returns>
        public int CalculateHeight(int parentHeight)
        {
            if (string.IsNullOrEmpty(this.SizeText))
            {
                return parentHeight;
            }

            if (this.SizeText[this.SizeText.Length - 1] == '%')
            {
                int percentage;
                if (!ParserUtil.TryParse(this.SizeText.Substring(0, this.SizeText.Length - 1), out percentage))
                {
                    return parentHeight;
                }

                return parentHeight * percentage / 100;
            }

            int size;
            if (!ParserUtil.TryParse(this.SizeText, out size))
            {
                return parentHeight;
            }

            return size;
        }
    }
}