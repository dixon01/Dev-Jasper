// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxVideoPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxVideoPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Video part for DirectX.
    /// </summary>
    public class DxVideoPart : DxPart, IVideoPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxVideoPart"/> class.
        /// </summary>
        /// <param name="videoUri">
        /// The video uri.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        public DxVideoPart(string videoUri, bool blink)
            : base(blink)
        {
            this.VideoUri = videoUri;
        }

        /// <summary>
        /// Gets the video URI.
        /// </summary>
        public string VideoUri { get; private set; }

        /// <summary>
        /// Updates the bounds of this part.
        /// </summary>
        /// <param name="sprite">
        ///     The sprite to use for calculations.
        /// </param>
        /// <param name="x">
        ///     The x position of the part within its parent.
        /// </param>
        /// <param name="y">
        ///     The y position of the part within its parent.
        /// </param>
        /// <param name="sizeFactor">
        ///     The resizing factor.
        /// </param>
        /// <param name="alignOnBaseline">
        ///     Value indicating if the current line of text has to be realigned on the base line.
        /// </param>
        /// <returns>
        /// The entire height of this part (even if it is not using all space).
        /// </returns>
        public override int UpdateBounds(Sprite sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            // TODO: implement
            alignOnBaseline = true;
            this.Bounds = new Rectangle(x, y, 0, 0);
            this.Ascent = this.Bounds.Height;
            return this.Bounds.Height;
        }

        /// <summary>
        /// Renders this part using the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="x">
        /// The absolute x position of the parent.
        /// </param>
        /// <param name="y">
        /// The absolute y position of the parent.
        /// </param>
        /// <param name="alpha">
        /// The alpha value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(Sprite sprite, int x, int y, int alpha, IDxRenderContext context)
        {
            // TODO: implement
        }
    }
}