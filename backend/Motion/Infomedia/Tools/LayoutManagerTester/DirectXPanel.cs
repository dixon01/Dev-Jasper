// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXPanel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXPanel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.LayoutManagerTester
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Panel showing text rendering in DirectX.
    /// </summary>
    public sealed class DirectXPanel : Panel
    {
        private RenderContext renderContext;
        private RenderManagerFactory renderManagerFactory;

        private Device device;

        private Line line;

        private ScreenRootRenderManager<IDxDeviceRenderContext> renderer;

        private ScreenRoot currentScreen;

        private int rotation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXPanel"/> class.
        /// </summary>
        public DirectXPanel()
        {
            this.BackColor = Color.Black;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            this.TextBounds = new Rectangle(20, 20, 100, 100);
        }

        /// <summary>
        /// Gets or sets the text bounds which will be displayed as a red rectangle.
        /// </summary>
        public Rectangle TextBounds { get; set; }

        /// <summary>
        /// Updates the text item being displayed.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void UpdateTextItem(TextItem item)
        {
            if (this.renderer != null)
            {
                this.renderer.Dispose();
                this.renderer = null;
            }

            this.currentScreen = new ScreenRoot { Root = new RootItem { Items = { item } } };
            this.rotation = item.Rotation;

            if (this.renderManagerFactory != null)
            {
                this.renderer = this.renderManagerFactory.CreateRenderManager(this.currentScreen);
            }

            this.renderContext.Reset();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.renderContext.Update();
            this.device.Clear(ClearFlags.Target, this.BackColor, 1.0f, 0);

            this.device.BeginScene();
            this.device.SetRenderState(RenderStates.AlphaBlendEnable, true);
            try
            {
                this.RenderTextBounds();
                this.RenderTextItem();
            }
            catch (Exception)
            {
            }

            this.device.EndScene();

            this.device.Present();

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.device != null)
            {
                return;
            }

            var presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            this.device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            // from http://www.vbforums.com/showthread.php?664787-RESOLVED-DirectX-Rotated-Text
            this.device.SetRenderState(RenderStates.ZEnable, true);
            this.device.SetRenderState(RenderStates.Lighting, 0);
            this.device.SetRenderState(RenderStates.CullMode, (int)Cull.CounterClockwise);
            this.device.SetRenderState(RenderStates.BlendOperation, (int)BlendOperation.Add);
            this.device.SetSamplerState(0, SamplerStageStates.MagFilter, (int)Filter.Linear);
            this.device.SetSamplerState(0, SamplerStageStates.MinFilter, (int)Filter.Linear);

            this.line = new Line(this.device);

            this.renderContext = new RenderContext(this.device);
            this.renderManagerFactory = new RenderManagerFactory(this.renderContext, new Rectangle(0, 0, 10000, 10000));

            if (this.currentScreen != null)
            {
                this.renderer = this.renderManagerFactory.CreateRenderManager(this.currentScreen);
            }
        }

        private void RenderTextItem()
        {
            if (this.renderer != null)
            {
                this.renderer.Render(1.0, this.renderContext);
            }
        }

        private void RenderTextBounds()
        {
            var rect = this.TextBounds;
            if (rect.Width > 0 || rect.Height > 0)
            {
                this.line.Draw(
                    new[]
                        {
                            this.Rotate(new Vector2(rect.Left, rect.Top)),
                            this.Rotate(new Vector2(rect.Right, rect.Top)),
                            this.Rotate(new Vector2(rect.Right, rect.Bottom)),
                            this.Rotate(new Vector2(rect.Left, rect.Bottom)),
                            this.Rotate(new Vector2(rect.Left, rect.Top))
                        },
                    Color.Red);
            }
            else
            {
                this.line.Draw(
                    new[] { new Vector2(rect.Left - 9, rect.Top), new Vector2(rect.Left + 10, rect.Top) }, Color.Red);
                this.line.Draw(
                    new[] { new Vector2(rect.Left, rect.Top - 9), new Vector2(rect.Left, rect.Top + 10) }, Color.Red);
            }
        }

        private Vector2 Rotate(Vector2 input)
        {
            var angle = this.rotation * Math.PI / 180;
            var centerX = this.TextBounds.X + (this.TextBounds.Width / 2);
            var centerY = this.TextBounds.Y + (this.TextBounds.Height / 2);
            return new Vector2(
                (float)(((input.X - centerX) * Math.Cos(angle)) - ((input.Y - centerY) * Math.Sin(angle)) + centerX),
                (float)(((input.Y - centerY) * Math.Cos(angle)) + ((input.X - centerX) * Math.Sin(angle)) + centerY));
        }

        private class RenderContext : IDxDeviceRenderContext
        {
            private readonly Dictionary<FontDescription, FontInfo> fonts =
                new Dictionary<FontDescription, FontInfo>(new FontDescriptionComparer());

            private int startCounter;

            public RenderContext(Device device)
            {
                this.Device = device;
                this.Reset();
            }

            public int MillisecondsCounter { get; private set; }

            public RendererConfig Config { get; private set; }

            public bool BlinkOn { get; private set; }

            public int AlternationCounter { get; private set; }

            public int ScrollCounter { get; private set; }

            public Device Device { get; private set; }

            public void Reset()
            {
                this.startCounter = Environment.TickCount;
            }

            public void Update()
            {
                this.MillisecondsCounter = Environment.TickCount;

                int diff = this.MillisecondsCounter - this.startCounter;
                this.AlternationCounter = diff / 3000;
                this.BlinkOn = ((diff / 500) % 2) == 0;
                this.ScrollCounter = diff;
            }

            public IImageTexture GetImageTexture(string filename)
            {
                using (var bitmap = new Bitmap(filename))
                {
                    return new ImageTexture(bitmap, this.Device);
                }
            }

            public IImageTexture GetImageTexture(Bitmap bitmap)
            {
                return new ImageTexture(bitmap, this.Device);
            }

            public void ReleaseImageTexture(IImageTexture texture)
            {
                var imageTexture = texture as ImageTexture;
                if (imageTexture == null)
                {
                    throw new ArgumentException("Expected ImageTexture");
                }

                imageTexture.Dispose();
            }

            public IFontInfo GetFontInfo(
                int height,
                int width,
                FontWeight weight,
                int mipLevels,
                bool italic,
                CharacterSet charSet,
                Precision outputPrecision,
                FontQuality quality,
                PitchAndFamily pitchAndFamily,
                string fontName)
            {
                var desc = new FontDescription
                {
                    Height = height,
                    Width = width,
                    Weight = weight,
                    MipLevels = mipLevels,
                    IsItalic = italic,
                    CharSet = charSet,
                    OutputPrecision = outputPrecision,
                    Quality = quality,
                    PitchAndFamily = pitchAndFamily,
                    FaceName = fontName
                };

                return this.GetFontInfo(desc);
            }

            public IFontInfo GetFontInfo(FontDescription description)
            {
                FontInfo font;
                if (!this.fonts.TryGetValue(description, out font))
                {
                    font = new FontInfo(this.Device, description);
                    font.Disposing += (s, e) => this.fonts.Remove(description);
                    this.fonts.Add(description, font);
                }

                return font;
            }
        }

        private class ImageTexture : IImageTexture, IDisposable
        {
            private readonly Texture texture;

            public ImageTexture(Bitmap bitmap, Device device)
            {
                this.texture = new Texture(device, bitmap, Usage.None, Pool.Managed);
                this.Size = bitmap.Size;
            }

            public Size Size { get; private set; }

            public void DrawTo(
                Sprite sprite, Rectangle srcRectangle, SizeF destinationSize, PointF position, Color color)
            {
                this.PrepareTexture(sprite.Device);
                sprite.Draw2D(this.texture, srcRectangle, destinationSize, position, color);
            }

            public void DrawTo(
                Sprite sprite,
                Rectangle srcRectangle,
                SizeF destinationSize,
                PointF rotationCenter,
                float rotationAngle,
                PointF position,
                Color color)
            {
                this.PrepareTexture(sprite.Device);
                sprite.Draw2D(
                    this.texture, srcRectangle, destinationSize, rotationCenter, rotationAngle, position, color);
            }

            public void Dispose()
            {
                if (this.texture.Disposed)
                {
                    return;
                }

                this.texture.Dispose();
            }

            private void PrepareTexture(Device device)
            {
                device.SetTexture(0, this.texture);

                device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                device.SetTextureStageState(0, TextureStageStates.ColorArgument2, (int)TextureArgument.Diffuse);
                device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.Modulate);

                device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
                device.SetTextureStageState(0, TextureStageStates.AlphaArgument2, (int)TextureArgument.Diffuse);
                device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
            }
        }

        private class FontDescriptionComparer : IEqualityComparer<FontDescription>
        {
            public bool Equals(FontDescription x, FontDescription y)
            {
                return x.Height == y.Height
                    && x.Width == y.Width
                    && x.MipLevels == y.MipLevels
                    && x.IsItalic == y.IsItalic
                    && x.CharSet == y.CharSet
                    && x.OutputPrecision == y.OutputPrecision
                    && x.Quality == y.Quality
                    && x.PitchAndFamily == y.PitchAndFamily
                    && x.FaceName == y.FaceName;
            }

            public int GetHashCode(FontDescription obj)
            {
                return obj.Height ^ obj.Weight.GetHashCode() ^ obj.FaceName.GetHashCode();
            }
        }
    }
}
