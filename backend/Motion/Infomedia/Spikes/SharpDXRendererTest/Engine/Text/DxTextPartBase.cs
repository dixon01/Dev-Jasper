// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTextPartBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTextPartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text
{
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Font = Gorba.Motion.Infomedia.Entities.Font;

    /// <summary>
    /// Base class for all Managed DirectX text parts.
    /// </summary>
    public abstract class DxTextPartBase : DxPart, ITextPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxTextPartBase"/> class.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="font">
        ///     The font.
        /// </param>
        /// <param name="blink">
        ///     The blink.
        /// </param>
        /// <param name="device">
        ///     The device.
        /// </param>
        protected DxTextPartBase(string text, Font font, bool blink, Device device)
            : base(blink, device)
        {
            this.Text = text;
            this.Font = font;

            if (font == null)
            {
                return;
            }

            var color = font.GetColor();
            this.Color = new Color(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color { get; private set; }
    }
}