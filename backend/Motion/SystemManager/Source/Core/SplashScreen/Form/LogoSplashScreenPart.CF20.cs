// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoSplashScreenPart.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogoSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Splash screen part that shows a logo.
    /// </summary>
    public partial class LogoSplashScreenPart
    {
        private void DrawImage(Graphics graphics, int x, int y)
        {
            var attrs = new ImageAttributes();
            if (this.useDefaultLogo)
            {
                attrs.SetColorKey(Color.Black, Color.Black);
            }

            graphics.DrawImage(
                this.bitmap,
                new Rectangle(x, y, this.size.Width, this.size.Height),
                0,
                0,
                this.bitmap.Width,
                this.bitmap.Height,
                GraphicsUnit.Pixel,
                attrs);
        }
    }
}