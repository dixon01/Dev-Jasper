// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XnaTextPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XnaTextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using System;
    using System.IO;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Color = System.Drawing.Color;

    public class XnaTextPart : XnaPart, ITextPart
    {
        private Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="XnaTextPart"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="blink">
        /// The blink.
        /// </param>
        public XnaTextPart(string text, Font font, bool blink)
            : base(blink)
        {
            this.Text = text;
            this.Font = font;

            if (font == null)
            {
                return;
            }

            this.color = font.GetColor();
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
        /// Gets or sets XnaFont.
        /// </summary>
        public SpriteFont XnaFont { get; set; }

        /// <summary>
        /// Gets Color.
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.Color Color { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="sizeFactor">
        /// The size factor.
        /// </param>
        /// <param name="alignOnBaseline">
        /// The align on baseline.
        /// </param>
        /// <returns>
        /// </returns>
        public override int UpdateBounds(SpriteBatch sprite, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            // To create Sprite Font and update bounds for the text part 
            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(SpriteBatch sprite, int x, int y, int alpha, IXnaRenderContext context)
        {
            this.Color = new Microsoft.Xna.Framework.Graphics.Color(this.color.R, this.color.G, this.color.B, alpha);

            sprite.DrawString(this.XnaFont, this.Text, new Vector2(x + this.Bounds.X, y + this.Bounds.Y), this.Color);
        }
    }
}
