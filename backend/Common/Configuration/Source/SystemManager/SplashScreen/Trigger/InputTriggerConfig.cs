// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputTriggerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputTriggerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Trigger to show or hide the splash screen when a GIOoM input changes its state.
    /// </summary>
    [Serializable]
    public class InputTriggerConfig : SplashScreenTriggerConfigBase
    {
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

        /// <summary>
        /// Gets or sets the value for which to trigger the change of splash screen.
        /// By default this property is null, meaning the trigger will react on any
        /// change of the given input. If this property is set, the trigger will only
        /// react when the input changes its value to the given (integer) value.
        /// </summary>
        [XmlIgnore]
        public int? Value { get; set; }

        /// <summary>
        /// Gets or sets the value for which to trigger the change of splash screen as an XML serializable string.
        /// </summary>
        [XmlAttribute("Value")]
        public string ValueString
        {
            get
            {
                return this.Value.HasValue ? this.Value.ToString() : string.Empty;
            }

            set
            {
                int intValue;
                if (value != null && ParserUtil.TryParse(value, out intValue))
                {
                    this.Value = intValue;
                }
                else
                {
                    this.Value = null;
                }
            }
        }
    }
}