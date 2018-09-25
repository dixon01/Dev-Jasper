// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiStatusConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiStatusConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.MultiStatus
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The root element of the multi-status file received from ScreenGate.
    /// </summary>
    [XmlRoot("projects", Namespace = "http://www.spinetix.com/namespace/1.0/spxproj")]
    public class MultiStatusConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStatusConfiguration"/> class.
        /// </summary>
        public MultiStatusConfiguration()
        {
            this.MultiStatuses = new List<MultiStatus>();
        }

        /// <summary>
        /// Gets or sets the list of multi-status objects.
        /// </summary>
        [XmlElement("multistatus", typeof(MultiStatus), Namespace = "DAV:")]
        public List<MultiStatus> MultiStatuses { get; set; }
    }
}
