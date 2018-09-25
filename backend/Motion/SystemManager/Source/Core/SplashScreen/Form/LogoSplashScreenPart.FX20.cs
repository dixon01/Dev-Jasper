// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoSplashScreenPart.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
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
            graphics.DrawImage(this.bitmap, x, y, this.size.Width, this.size.Height);
        }
    }
}