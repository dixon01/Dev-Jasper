// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the application restart dependency on a folder update
    /// </summary>
    [Serializable]
    public class DependencyConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyConfig"/> class.
        /// </summary>
        public DependencyConfig()
        {
            this.Path = string.Empty;
            this.ExecutablePaths = new List<string>();
        }

        /// <summary>
        /// Gets or sets the path of the folder or file whose update will trigger the restart of the
        /// application(s) available at the <see cref="ExecutablePaths"/>.
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the list of executable paths of the applications to be restarted if an update
        /// of the folder present in the path occurs.
        /// </summary>
        [XmlElement("ExecutablePath")]
        public List<string> ExecutablePaths { get; set; }
    }
}
