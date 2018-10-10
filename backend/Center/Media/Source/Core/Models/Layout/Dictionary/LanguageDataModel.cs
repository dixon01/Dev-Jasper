// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout.Dictionary
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the DataModel for a dictionary language that can be serialized.
    /// </summary>
    [XmlRoot("Language")]
    [Serializable]
    public class LanguageDataModel
    {
        /// <summary>
        /// Gets or sets the language index.
        /// </summary>
        [XmlAttribute("Index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the language
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}
