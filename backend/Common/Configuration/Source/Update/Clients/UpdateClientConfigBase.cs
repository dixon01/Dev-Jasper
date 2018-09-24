// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateClientConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Clients
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for all update clients
    /// </summary>
    [Serializable]
    public abstract class UpdateClientConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateClientConfigBase"/> class.
        /// </summary>
        protected UpdateClientConfigBase()
        {
            this.ShowVisualization = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress is to be shown by Update.
        /// Default value is true
        /// </summary>
        [XmlElement]
        public bool ShowVisualization { get; set; }

        /// <summary>
        /// Gets or sets the unique name of this update client.
        /// If this name changes, all cached data for this client
        /// will be lost.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

   }
}
