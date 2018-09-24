// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxImagePart.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxImagePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Image part for DirectX.
    /// </summary>
    public class DxImagePart : DxPart, IImagePart
    {
        private readonly ImagePartSprite imageSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxImagePart"/> class.
        /// </summary>
        /// <param name="fileName">
        ///     The image file name.
        /// </param>
        /// <param name="blink">
        ///     The blink.
        /// </param>
        /// <param name="context">
        ///     The render context.
        /// </param>
        public DxImagePart(string fileName, bool blink, IDxDeviceRenderContext context)
            : base(blink, context)
        {
            this.FileName = fileName;

            this.imageSprite = new ImagePartSprite(context);
            this.imageSprite.Setup(this.FileName);
        }

        /// <summary>
        /// Gets the image file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new DxImagePart(this.FileName, this.Blink, this.Context);
        }

        /// <summary>
        /// Sets the scaling factor of this part.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public override void SetScaling(double factor)
        {
            var width = (int)(this.imageSprite.Size.Width * factor);
            var height = (int)(this.imageSprite.Size.Height * factor);
            this.SetSize(width, height, height);
        }

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
        public override bool Split(int offset, out DxPart left, out DxPart right)
        {
            left = this;
            right = null;
            return false;
        }

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
        public override void Render(
            int x, int y, PointF rotationCenter, float rotationAngle, int alpha, IDxRenderContext context)
        {
            var bounds = new Rectangle(x + this.X, y + this.Y, this.Width, this.Height);
            this.imageSprite.Render(bounds, rotationCenter, rotationAngle, alpha, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.imageSprite != null)
            {
                this.imageSprite.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public override void OnResetDevice()
        {
            base.OnResetDevice();

            if (this.imageSprite != null)
            {
                this.imageSprite.OnResetDevice();
            }
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public override void OnLostDevice()
        {
            base.OnLostDevice();

            if (this.imageSprite != null)
            {
                this.imageSprite.OnLostDevice();
            }
        }

        private class ImagePartSprite : ImageSprite
        {
            private PointF currentRotationCenter;
            private float currentRotationAngle;

            public ImagePartSprite(IDxDeviceRenderContext context)
                : base(context)
            {
            }

            public void Render(
                Rectangle bounds, PointF rotationCenter, float rotationAngle, int alpha, IDxRenderContext renderContext)
            {
                this.currentRotationCenter = rotationCenter;
                this.currentRotationAngle = rotationAngle;
                this.Render(bounds, ElementScaling.Stretch, alpha, renderContext);
            }

            protected override void DrawTexture(
                Sprite destinationSprite,
                IImageTexture srcTexture,
                Rectangle srcRectangle,
                SizeF destinationSize,
                PointF position,
                Color color)
            {
                srcTexture.DrawTo(
                    destinationSprite,
                    srcRectangle,
                    destinationSize,
                    new PointF(
                        (this.currentRotationCenter.X - position.X) / destinationSize.Width * srcRectangle.Width,
                        (this.currentRotationCenter.Y - position.Y) / destinationSize.Height * srcRectangle.Height),
                    this.currentRotationAngle,
                    this.currentRotationCenter,
                    color);
            }
        }
    }
}