// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleVideoPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleVideoPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    /// <summary>
    /// Video part created by <see cref="SimpleTextFactory"/>.
    /// </summary>
    public class SimpleVideoPart : SimplePartBase, IVideoPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleVideoPart"/> class.
        /// </summary>
        /// <param name="videoUri">
        /// The video URI.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        internal SimpleVideoPart(string videoUri, bool blink)
            : base(blink)
        {
            this.VideoUri = videoUri;
        }

        /// <summary>
        /// Gets the video URI.
        /// </summary>
        public string VideoUri { get; private set; }
    }
}