// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListElementDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the properties of an image list layout element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the properties of an image list element.
    /// </summary>
    public partial class ImageListElementDataModel
    {
        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [XmlAttribute("TestData")]
        public string TestData { get; set; }
    }
}
