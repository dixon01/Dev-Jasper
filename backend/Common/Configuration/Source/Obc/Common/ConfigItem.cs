// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Common
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The config item.
    /// </summary>
    /// <typeparam name="TConfigValue">
    /// The type of the config value.
    /// </typeparam>
    [Serializable]
    public class ConfigItem<TConfigValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigItem{TConfigValue}"/> class.
        /// </summary>
        public ConfigItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigItem{TConfigValue}"/> class.
        /// </summary>
        /// <param name="itemValue">Value for config item</param>
        /// <param name="description">Description for config item</param>
        public ConfigItem(TConfigValue itemValue, string description)
        {
            this.Value = itemValue;
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the description of config item (deserialization will overwrite)
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value of config item
        /// </summary>
        [XmlElement(ElementName = "Value")]
        public TConfigValue Value { get; set; }

        /// <summary>
        /// Overwritten cast operator
        /// </summary>
        /// <param name="configItem">ConfigItem to be casted</param>
        /// <returns>Generic type of value of ConfigItem</returns>
        public static implicit operator TConfigValue(ConfigItem<TConfigValue> configItem)
        {
            // explicit cast logic
            return configItem.Value;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}