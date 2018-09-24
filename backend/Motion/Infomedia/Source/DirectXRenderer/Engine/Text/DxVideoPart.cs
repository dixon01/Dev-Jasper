// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxVideoPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxVideoPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// Video part for DirectX.
    /// </summary>
    public class DxVideoPart : DxPart, IVideoPart
    {
        private readonly VideoSpriteBase videoSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxVideoPart"/> class.
        /// </summary>
        /// <param name="videoUri">
        ///     The video uri.
        /// </param>
        /// <param name="blink">
        ///     The blink.
        /// </param>
        /// <param name="context">
        /// The render context.
        /// </param>
        public DxVideoPart(string videoUri, bool blink, IDxDeviceRenderContext context)
            : base(blink, context)
        {
            this.VideoUri = videoUri;

            this.videoSprite = VideoSpriteBase.Create(context);
            this.videoSprite.Setup(this.VideoUri, this.FallbackImage);
        }

        /// <summary>
        /// Gets the fallback image.
        /// </summary>
        public string FallbackImage { get; private set; }

        /// <summary>
        /// Gets the video URI.
        /// </summary>
        public string VideoUri { get; private set; }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new DxVideoPart(this.VideoUri, this.Blink, this.Context);
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
            var width = (int)(this.videoSprite.Size.Width * factor);
            var height = (int)(this.videoSprite.Size.Height * factor);
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
            this.videoSprite.Render(bounds, ElementScaling.Stretch, alpha, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.videoSprite != null)
            {
                this.videoSprite.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public override void OnResetDevice()
        {
            base.OnResetDevice();

            if (this.videoSprite != null)
            {
                this.videoSprite.OnResetDevice();
            }
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public override void OnLostDevice()
        {
            base.OnLostDevice();

            if (this.videoSprite != null)
            {
                this.videoSprite.OnLostDevice();
            }
        }
    }
}