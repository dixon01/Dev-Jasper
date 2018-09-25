// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Language.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Generic
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// This class contains the language specific tables and columns of the <see cref="Dictionary"/>.
    /// </summary>
    [Serializable]
    public class Language : IDictionaryItem
    {
        /// <summary>
        /// Gets or sets the language index.
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the language
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the language
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }
    }
}
