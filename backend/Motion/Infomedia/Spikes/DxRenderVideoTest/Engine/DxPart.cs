// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Base class for all DirectX formatted text parts.
    /// </summary>
    public abstract class DxPart : IPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxPart"/> class.
        /// </summary>
        /// <param name="blink">
        /// A value indicating whether this part should blink.
        /// </param>
        protected DxPart(bool blink)
        {
            this.Blink = blink;
        }

        /// <summary>
        /// Gets a value indicating whether this part should blink.
        /// </summary>
        public bool Blink { get; private set; }

        /// <summary>
        /// Gets or sets the bounds of this part.
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets or sets the ascent (pixels above base-line).
        /// </summary>
        public int Ascent { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a new line.
        /// </summary>
        public virtual bool IsNewLine
        {
            get
            {
                return false;
            }
        }

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
        public abstract int UpdateBounds(Sprite sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline);

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
        public abstract void Render(Sprite sprite, int x, int y, int alpha, IDxRenderContext context);

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public virtual void OnResetDevice(Device device)
        {
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public virtual void OnLostDevice()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }
}