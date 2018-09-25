// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogoSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System.Drawing;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;

    /// <summary>
    /// Splash screen part that shows a logo.
    /// </summary>
    public partial class LogoSplashScreenPart : SplashScreenPartBase
    {
        private readonly Bitmap bitmap;

        // ReSharper disable NotAccessedField.Local
        // reason: used by CF 3.5 code
        private readonly bool useDefaultLogo;

        // ReSharper restore NotAccessedField.Local
        private Size size;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public LogoSplashScreenPart(LogoSplashScreenItem config)
        {
            if (string.IsNullOrEmpty(config.Filename))
            {

               
#if __UseLuminatorTftDisplay
                this.bitmap = Resources.LuminatorTransparency;
#else
                 this.bitmap = Resources.GorbaLogoTransparency;
#endif
                this.useDefaultLogo = true;
            }
            else
            {
                this.bitmap = new Bitmap(config.Filename);
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
            var x = (rect.Width - this.size.Width) / 2;
            this.DrawImage(g, x, rect.Y);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();
            this.bitmap.Dispose();
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
            this.size.Width = (int)(this.bitmap.Width * factor);
            this.size.Height = (int)(this.bitmap.Height * factor);
            return this.size;
        }
    }
}