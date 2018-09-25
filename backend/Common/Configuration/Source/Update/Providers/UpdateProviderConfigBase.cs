// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProviderConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Providers
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all update provider configurations.
    /// </summary>
    [Serializable]
    public abstract class UpdateProviderConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProviderConfigBase"/> class.
        /// </summary>
        protected UpdateProviderConfigBase()
        {
            this.ShowVisualization = true;
        }

        /// <summary>
        /// Gets or sets the unique name of this update provider.
        /// If this name changes, all cached data for this provider
        /// will be lost.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the progress is to be shown by Update.
        /// Default value is true
        /// </summary>
        [XmlElement]
        public bool ShowVisualization { get; set; }
    }
}
