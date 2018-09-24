// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsSplashScreenVisibilityBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationsSplashScreenVisibilityBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// Base class to hide or show the given application on the splash screen.
    /// </summary>
    [Serializable]
    public abstract class ApplicationsSplashScreenVisibilityBase
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// This has to match a <see cref="ApplicationConfigBase.Name"/>.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }
    }
}