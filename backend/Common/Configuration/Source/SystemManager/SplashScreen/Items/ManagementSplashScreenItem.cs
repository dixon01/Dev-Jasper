// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementSplashScreenItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Show the value of a management property on the splash screen.
    /// </summary>
    public class ManagementSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementSplashScreenItem"/> class.
        /// </summary>
        public ManagementSplashScreenItem()
        {
            this.ValueFormat = "{0}";
        }

        /// <summary>
        /// Gets or sets the label to be shown next to the I/O value.
        /// If this property is left empty, the last part of the <see cref="Path"/> will be used.
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
        /// Gets or sets the unit name where to find the management information.
        /// By default this property is empty, meaning the local system is used.
        /// </summary>
        [XmlAttribute]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the application name where to find the management information.
        /// By default this property is empty, meaning the current application is used.
        /// If the <see cref="Unit"/> is set, then also this property has to be set.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the path to the management property.
        /// The path is delimited by \ and the last part of has to be the name of the property.
        /// This property is mandatory.
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the interval at which to update the value of the management object.
        /// If the value is not set, the value is only read once and never updated.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? UpdateInterval { get; set; }

        /// <summary>
        /// Gets or sets the interval at which to update the value as an XML string.
        /// </summary>
        [XmlAttribute("UpdateInterval", DataType = "duration")]
        public string UpdateIntervalString
        {
            get
            {
                return this.UpdateInterval.HasValue ? XmlConvert.ToString(this.UpdateInterval.Value) : null;
            }

            set
            {
                this.UpdateInterval = string.IsNullOrEmpty(value) ? null : (TimeSpan?)XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
