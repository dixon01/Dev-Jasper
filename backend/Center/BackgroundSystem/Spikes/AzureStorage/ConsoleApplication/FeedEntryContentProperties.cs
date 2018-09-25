// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedEntryContentProperties.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedEntryContentProperties type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ConsoleApplication
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The feed entry content properties.
    /// </summary>
    [XmlRoot("properties", Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
    public class FeedEntryContentProperties
    {
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        [XmlAnyElement]
        public XmlElement[] Values { get; set; }

        /// <summary>
        /// Gets a value.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The property was not found.
        /// </exception>
        public T GetValue<T>(string name)
        {
            foreach (var property in this.Values)
            {
                if (string.Equals(property.LocalName, name, StringComparison.InvariantCulture))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    return (T)converter.ConvertFromString(property.Value);
                }
            }

            throw new Exception("Property not found");
        }
    }
}