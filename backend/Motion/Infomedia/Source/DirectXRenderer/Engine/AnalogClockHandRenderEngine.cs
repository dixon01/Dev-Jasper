// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockHandRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The analog clock hand render engine.
    /// </summary>
    public class AnalogClockHandRenderEngine
        : RenderEngineBase<AnalogClockHandItem,
            IAnalogClockHandRenderEngine<IDxDeviceRenderContext>,
            AnalogClockHandRenderManager<IDxDeviceRenderContext>>,
          IAnalogClockHandRenderEngine<IDxDeviceRenderContext>
    {
        private HandImageSprite imageSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogClockHandRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public AnalogClockHandRenderEngine(AnalogClockHandRenderManager<IDxDeviceRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(double alpha, IDxDeviceRenderContext context)
        {
            if (this.imageSprite == null)
            {
                this.imageSprite = new HandImageSprite(this.Manager, context);
            }

            var bounds = this.Manager.Bounds;
            bounds.X += this.Viewport.X;
            bounds.Y += this.Viewport.Y;

            this.imageSprite.Setup(this.Manager.Filename);
            this.imageSprite.Render(bounds, ElementScaling.Stretch, (int)(255 * alpha), context);
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        protected override void OnResetDevice()
        {
            base.OnResetDevice();
            if (this.imageSprite != null)
            {
                this.imageSprite.OnResetDevice();
            }
        }

        /// <summary>
        /// Override this method to be notified when a device is lost.
        /// If you have any DirectX resources, make sure to call
        /// OnLostDevice() on them if available.
        /// </summary>
        protected override void OnLostDevice()
        {
            base.OnLostDevice();
            if (this.imageSprite != null)
            {
                this.imageSprite.OnLostDevice();
            }
        }

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
        protected override void Release()
        {
            if (this.imageSprite != null)
            {
                this.imageSprite.Dispose();
                this.imageSprite = null;
            }
        }

        private class HandImageSprite : ImageSprite
        {
            private readonly AnalogClockHandRenderManager<IDxDeviceRenderContext> manager;

            public HandImageSprite(
                AnalogClockHandRenderManager<IDxDeviceRenderContext> manager, IDxDeviceRenderContext context)
                : base(context)
            {
                this.manager = manager;
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
                    new PointF(this.manager.CenterX, this.manager.CenterY),
                    (float)this.manager.Rotation,
                    position,
                    color);
            }
        }
    }
}