// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleImagePart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleImagePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    /// <summary>
    /// Image part created by <see cref="SimpleTextFactory"/>.
    /// </summary>
    public class SimpleImagePart : SimplePartBase, IImagePart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleImagePart"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The image file name.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        internal SimpleImagePart(string fileName, bool blink)
            : base(blink)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets the image file name.
        /// </summary>
        public string FileName { get; private set; }
    }
}