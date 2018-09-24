// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTextElementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the properties of a dynamic text layout element.
    /// </summary>
    [XmlRoot("DynamicText")]
    [Serializable]
    public class DynamicTextElementDataModel : TextElementDataModel
    {
        /// <summary>
        /// Gets or sets the dynamic text.
        /// </summary>
        [XmlElement("DictionaryValue")]
        public DictionaryValueElementDataModel SelectedDictionaryValue { get; set; }

        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [XmlAttribute("TestData")]
        public string TestData { get; set; }
    }
}
