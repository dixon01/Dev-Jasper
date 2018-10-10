// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSplashScreenPartBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextSplashScreenPartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Base class for splash screen parts that show a simple string on the screen (no additional formatting).
    /// </summary>
    public abstract class TextSplashScreenPartBase : SplashScreenPartBase
    {
        private Font font;

        private SolidBrush brush;

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
                this.brush = new SolidBrush(value);
            }
        }

        /// <summary>
        /// Paints this part.
        /// </summary>
        /// <param name="g">
        /// The graphics object.
        /// </param>
        /// <param name="rect">
        /// The rectangle into which we should paint.
        /// </param>
        public override void Paint(Graphics g, Rectangle rect)
        {
            var x = rect.X + (rect.Width / 2);
            var y = rect.Y;
            g.DrawString(
                this.GetDisplayString(),
                this.font,
                this.brush,
                x,
                y,
                new StringFormat { Alignment = StringAlignment.Center });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.font != null)
            {
                this.font.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// Implementation of the scaling.
        /// </summary>
        /// <param name="factor">
        /// The factor (0.0 .. 1.0).
        /// </param>
        /// <param name="graphics">
        /// The graphics to calculate the scaling.
        /// </param>
        /// <returns>
        /// The calculated height of this part with the used scaling factor.
        /// </returns>
        protected override Size DoScale(double factor, Graphics graphics)
        {
            if (this.font != null)
            {
                this.font.Dispose();
            }

            this.font = new Font(FontFamily.GenericSansSerif, (float)(24 * factor), FontStyle.Bold);

            var size = graphics.MeasureString(this.GetDisplayString(), this.font);
            return new Size
                       {
                           Height = (int)Math.Ceiling(size.Height),
                           Width = (int)size.Width
                       };
        }

        /// <summary>
        /// Calculates the string to be shown on the splash screen.
        /// This string can contain multiple lines.
        /// </summary>
        /// <returns>
        /// The string to be shown on the splash screen.
        /// </returns>
        protected abstract string GetDisplayString();
    }
}