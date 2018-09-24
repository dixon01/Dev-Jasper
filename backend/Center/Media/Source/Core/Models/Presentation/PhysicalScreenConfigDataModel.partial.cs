// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenConfigDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
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
    public partial class PhysicalScreenConfigDataModel
    {
        /// <summary>
        /// Gets or sets the selected master layout.
        /// </summary>
        [XmlElement("SelectedMasterLayout")]
        public MasterLayout SelectedMasterLayout { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is monochrome (only relevant for LED screens).
        /// </summary>
        [XmlElement("IsMonochromeScreen")]
        public bool IsMonochromeScreen { get; set; }
    }
}
