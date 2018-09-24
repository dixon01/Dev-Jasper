namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Manages the intertion point when drawing text
    /// </summary>
    public struct TextHelper
    {
        private Font textFont; // Used to draw the text
        private Sprite textSprite; // Used to cache the drawn text
        private int color; // Color to draw the text
        private System.Drawing.Point point; // Where to draw the text
        private int lineHeight; // Height of the lines

        /// <summary>
        /// Create a new instance of the text helper class
        /// </summary>
        public TextHelper(Font f, Sprite s, int l)
        {
            this.textFont = f;
            this.textSprite = s;
            this.lineHeight = l;
            this.color = unchecked((int)0xffffffff);
            this.point = System.Drawing.Point.Empty;
        }

        /// <summary>
        /// Draw a line of text
        /// </summary>
        public void DrawTextLine(string text)
        {
            if (this.textFont == null)
            {
                throw new InvalidOperationException("You cannot draw text.  There is no font object.");
            }
            // Create the rectangle to draw to
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(this.point, System.Drawing.Size.Empty);
            this.textFont.DrawText(this.textSprite, text, rect, DrawTextFormat.NoClip, this.color);

            // Increase the line height
            this.point.Y += this.lineHeight;
        }

        /// <summary>
        /// Draw a line of text
        /// </summary>
        public void DrawTextLine(string text, params object[] args)
        {
            // Simply format the string and pass it on
            this.DrawTextLine(string.Format(text, args));
        }

        /// <summary>
        /// Insertion point of the text
        /// </summary>
        public void SetInsertionPoint(System.Drawing.Point p) { this.point = p; }
        public void SetInsertionPoint(int x, int y) { this.point.X = x; this.point.Y = y; }

        /// <summary>
        /// The color of the text
        /// </summary>
        public void SetForegroundColor(int c) { this.color = c; }
        public void SetForegroundColor(System.Drawing.Color c) { this.color = c.ToArgb(); }

        /// <summary>
        /// Begin the sprite rendering
        /// </summary>
        public void Begin()
        {
            if (this.textSprite != null)
            {
                this.textSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
            }
        }

        /// <summary>
        /// End the sprite
        /// </summary>
        public void End()
        {
            if (this.textSprite != null)
            {
                this.textSprite.End();
            }
        }
    }
}