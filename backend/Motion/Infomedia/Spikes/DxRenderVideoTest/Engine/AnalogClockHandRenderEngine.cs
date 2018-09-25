// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockHandRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The analog clock hand render engine.
    /// </summary>
    public class AnalogClockHandRenderEngine
        : RenderEngineBase<AnalogClockItem.Hand, IAnalogClockHandRenderEngine<IDxRenderContext>, AnalogClockRenderManager<IDxRenderContext>.Hand>,
          IAnalogClockHandRenderEngine<IDxRenderContext>
    {
        private ImageSprite imageSprite;

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
        public override void Render(double alpha, IDxRenderContext context)
        {
            if (this.imageSprite == null)
            {
                this.imageSprite = new ImageSprite(this.Manager, this.Device);
            }

            this.imageSprite.Setup(this.Manager.Filename);
            this.imageSprite.Render((int)(255 * alpha), context);
        }

        /// <summary>
        /// Override this method to be notified when a new device is created.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected override void OnCreateDevice(Device device)
        {
            this.Release();
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        /// <param name="dev">
        /// The device.
        /// </param>
        protected override void OnResetDevice(Device dev)
        {
            base.OnResetDevice(dev);
            if (this.imageSprite != null)
            {
                this.imageSprite.OnResetDevice(dev);
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

        private class ImageSprite : ImageSprite<AnalogClockItem.Hand, IAnalogClockHandRenderEngine<IDxRenderContext>>
        {
            private readonly AnalogClockRenderManager<IDxRenderContext>.Hand manager;

            public ImageSprite(AnalogClockRenderManager<IDxRenderContext>.Hand manager, Device device)
                : base(manager, device)
            {
                this.manager = manager;
            }

            protected override void DrawTexture(Sprite destinationSprite, Texture srcTexture, Rectangle srcRectangle, SizeF destinationSize, PointF position, Color color)
            {
                destinationSprite.Draw2D(
                    srcTexture,
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