// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenPartBase.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationsSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System.Drawing;

    /// <summary>
    /// Base class for visual items shown on a splash screen.
    /// </summary>
    public partial class SplashScreenPartBase
    {
        /// <summary>
        /// The default string format for rendering strings on the screen.
        /// </summary>
        protected static readonly StringFormat DefaultStringFormat = new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap
            };
    }
}