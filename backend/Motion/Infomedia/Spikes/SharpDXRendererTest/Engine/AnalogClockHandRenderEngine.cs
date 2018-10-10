// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockHandRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Image;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Rectangle = System.Drawing.Rectangle;

    /// <summary>
    /// The analog clock hand render engine.
    /// </summary>
    public class AnalogClockHandRenderEngine
        : RenderEngineBase<AnalogClockItem.Hand, IAnalogClockHandRenderEngine<IDxRenderContext>, AnalogClockRenderManager<IDxRenderContext>.Hand>,
          IAnalogClockHandRenderEngine<IDxRenderContext>
    {
        private HandImageSprite imageSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogClockHandRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public AnalogClockHandRenderEngine(AnalogClockRenderManager<IDxRenderContext>.Hand manager)
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
        protected override void DoRender(double alpha, IDxRenderContext context)
        {
            if (this.imageSprite == null)
            {
                this.imageSprite = new HandImageSprite(this.Manager, this.Device);
            }

            var bounds = this.Manager.Bounds;
            bounds.X -= this.Viewport.X;
            bounds.Y -= this.Viewport.Y;

            this.imageSprite.Setup(this.Manager.Filename);
            this.imageSprite.Render(bounds, (int)(255 * alpha), context);
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
            private readonly AnalogClockRenderManager<IDxRenderContext>.Hand manager;

            public HandImageSprite(AnalogClockRenderManager<IDxRenderContext>.Hand manager, Device device)
                : base(device)
            {
                this.manager = manager;
            }

            protected override void DrawTexture(Sprite destinationSprite, Texture srcTexture, Rectangle srcRectangle, SizeF destinationSize, PointF position, ColorBGRA color)
            {
                /*destinationSprite.Draw(
                    srcTexture,
                    srcRectangle,
                    destinationSize,
                    new PointF(this.manager.CenterX, this.manager.CenterY), 
                    (float)this.manager.Rotation,
                    position,
                    color);*/
            }
        }
    }
}