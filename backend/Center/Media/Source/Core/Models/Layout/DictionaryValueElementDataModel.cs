// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryValueElementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models.Layout.Dictionary;

    /// <summary>
    /// The DictionaryValueElementDataModel.
    /// </summary>
    [XmlRoot("DictionaryValue")]
    [Serializable]
    public class DictionaryValueElementDataModel : DataModelBase
    {
        /// <summary>
        /// Gets or sets the <see cref="Column"/>.
        /// </summary>
        public ColumnDataModel Column { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Table"/>.
        /// </summary>
        public TableDataModel Table { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Language"/>.
        /// </summary>
        public LanguageDataModel Language { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        public int Row { get; set; }
    }
}
