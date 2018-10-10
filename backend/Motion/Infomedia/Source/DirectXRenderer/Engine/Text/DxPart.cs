// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase.Layouting;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// Base class for all DirectX formatted text parts.
    /// </summary>
    public abstract class DxPart : IPart, ILayoutItem, ISplittable<DxPart>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxPart"/> class.
        /// </summary>
        /// <param name="blink">
        /// A value indicating whether this part should blink.
        /// </param>
        /// <param name="context">
        /// The render context.
        /// </param>
        protected DxPart(bool blink, IDxDeviceRenderContext context)
        {
            this.Context = context;
            this.Blink = blink;
        }

        /// <summary>
        /// Gets a value indicating whether this part should blink.
        /// </summary>
        public bool Blink { get; private set; }

        /// <summary>
        /// Gets the X coordinate of this part.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate of this part.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the width of this part.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of this part.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the ascent (pixels above base-line).
        /// </summary>
        public int Ascent { get; private set; }

        /// <summary>
        /// Gets the horizontal gap after this item if it is adjacent to another item.
        /// This gap is not taken into account when there is no item coming after the current one,
        /// but it is added horizontally if there is a next item on the same line.
        /// </summary>
        public int HorizontalGapAfter
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the render context.
        /// </summary>
        protected IDxDeviceRenderContext Context { get; private set; }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public abstract IPart Duplicate();

        /// <summary>
        /// Moves this part to the given location.
        /// </summary>
        /// <param name="x">
        /// The new X coordinate of the part.
        /// </param>
        /// <param name="y">
        /// The new Y coordinate of the part.
        /// </param>
        public virtual void MoveTo(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Sets the scaling factor of this part.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public abstract void SetScaling(double factor);

        /// <summary>
        /// Tries to split the part into two parts at the given offset.
        /// </summary>
        /// <param name="offset">
        /// The offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left part of the split operation. This is never null.
        /// If the part couldn't be split, this return parameter might be the object this method was called on.
        /// </param>
        /// <param name="right">
        /// The right part of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        public abstract bool Split(int offset, out DxPart left, out DxPart right);

        /// <summary>
        /// Renders this part using the given sprite.
        /// </summary>
        /// <param name="x">
        /// The absolute x position of the parent.
        /// </param>
        /// <param name="y">
        /// The absolute y position of the parent.
        /// </param>
        /// <param name="rotationCenter">
        /// The absolut position around which this part should be rotated.
        /// </param>
        /// <param name="rotationAngle">
        /// The angle in radian at which should be rotated.
        /// </param>
        /// <param name="alpha">
        /// The alpha value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public abstract void Render(
            int x, int y, PointF rotationCenter, float rotationAngle, int alpha, IDxRenderContext context);

        /// <summary>
        /// Updates the contents of this part if necessary before rendering it.
        /// </summary>
        /// <returns>
        /// True if the content has been updated and thus the part (and probably the entire text)
        /// has to be recalculated.
        /// </returns>
        public virtual bool UpdateContent()
        {
            return false;
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public virtual void OnResetDevice()
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

        /// <summary>
        /// Sets the width, height and ascent of this part.
        /// This method must be called from within <see cref="SetScaling"/>
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="ascent">
        /// The ascent.
        /// </param>
        protected void SetSize(int width, int height, int ascent)
        {
            this.Width = width;
            this.Height = height;
            this.Ascent = ascent;
        }
    }
}