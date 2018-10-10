// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomSplashScreenItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Show the current value of a GIOoM port on the splash screen.
    /// </summary>
    [Serializable]
    public class GioomSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GioomSplashScreenItem"/> class.
        /// </summary>
        public GioomSplashScreenItem()
        {
            this.ValueFormat = "{0}";
        }

        /// <summary>
        /// Gets or sets the label to be shown next to the I/O value.
        /// If this property is left empty, the <see cref="Name"/> will be used.
        /// </summary>
        [XmlAttribute]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the value format string to be used when displaying the value.
        /// The '{0}' placeholder will be replaced with the value coming from the I/O.
        /// </summary>
        [XmlAttribute]
        [DefaultValue("{0}")]
        public string ValueFormat { get; set; }

        /// <summary>
        /// Gets or sets the unit name where to find the input.
        /// By default this property is empty, meaning the input will be searched on
        /// the local system only.
        /// </summary>
        [XmlAttribute]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the application name where to find the input.
        /// By default this property is empty, meaning the input will be searched in
        /// all applications.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the name of the input.
        /// This property is mandatory.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
    }
}
