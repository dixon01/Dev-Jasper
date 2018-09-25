// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItem.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    /// <summary>
    /// Container of the couple of information {XML tag's name, XML tag's value}.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// Gets or sets XML tag's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets XML tag's value.
        /// </summary>
        public string Value { get; set; }
    }
}
