// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplayConfigDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The physical screen config data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Presentation
{
    using System.Windows;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Configuration;

    /// <summary>
    /// The physical screen config data model.
    /// </summary>
    public partial class VirtualDisplayConfigDataModel
    {
        /// <summary>
        /// Gets or sets the current zoom level.
        /// </summary>
        [XmlElement("CurrentZoomLevel")]
        public double CurrentZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the current layout position.
        /// </summary>
        [XmlElement("CurrentLayoutPosition")]
        public Point CurrentLayoutPosition { get; set; }
    }
}
