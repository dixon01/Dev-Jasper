// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogoSplashScreenItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Show a logo on the splash screen.
    /// </summary>
    [Serializable]
    public class LogoSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Gets or sets the filename of the image to show.
        /// If this is null or the file doesn't exist, the default (Gorba) logo is shown.
        /// </summary>
        [XmlAttribute]
        public string Filename { get; set; }
    }
}