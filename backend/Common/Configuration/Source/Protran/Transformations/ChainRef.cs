// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChainRef.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChainRef type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Reference to another chain. This can be used
    /// to reuse a chain within another one.
    /// </summary>
    [Serializable]
    public class ChainRef : TransformationConfig
    {
        /// <summary>
        /// Gets or sets the reference to the chain.
        /// This has to be a valid chain ID.
        /// </summary>
        [XmlAttribute]
        public string TransfRef { get; set; }
    }
}
