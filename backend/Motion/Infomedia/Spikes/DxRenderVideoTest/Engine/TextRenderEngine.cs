// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The text render engine.
    /// </summary>
    public class TextRenderEngine
        : RenderEngineBase<TextItem, ITextRenderEngine<IDxRenderContext>, TextRenderManager<IDxRenderContext>>,
          ITextRenderEngine<IDxRenderContext>
    {
        private TextSprite oldSprite;
        private TextSprite newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public TextRenderEngine(TextRenderManager<IDxRenderContext> manager)
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
            if (this.oldSprite == null)
            {
                this.oldSprite = new TextSprite(this.Manager, this.Device);
            }

            if (this.newSprite == null)
            {
                this.newSprite = new TextSprite(this.Manager, this.Device);
            }

            var oldText = this.Manager.Text.OldValue;
            var newText = this.Manager.Text.NewValue;

            if (oldText != null)
            {
                if (this.oldSprite.CurrentText != oldText
                    && this.newSprite.CurrentText == oldText)
                {
                    // swap sprites
                    var sprite = this.oldSprite;
                    this.oldSprite = this.newSprite;
                    this.newSprite = sprite;
                }

                this.oldSprite.Setup(oldText);
                this.oldSprite.Render((int)(255 * alpha * this.Manager.Text.OldAlpha), context);
            }

            if (newText != null)
            {
                this.newSprite.Setup(newText);
                this.newSprite.Render((int)(255 * alpha * this.Manager.Text.NewAlpha), context);
            }
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
            if (this.oldSprite != null)
            {
                this.oldSprite.OnResetDevice(dev);
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnResetDevice(dev);
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
            if (this.oldSprite != null)
            {
                this.oldSprite.OnLostDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnLostDevice();
            }
        }

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
        protected override void Release()
        {
            if (this.oldSprite != null)
            {
                this.oldSprite.Dispose();
                this.oldSprite = null;
            }

            if (this.newSprite != null)
            {
                this.newSprite.Dispose();
                this.newSprite = null;
            }
        }
    }
}