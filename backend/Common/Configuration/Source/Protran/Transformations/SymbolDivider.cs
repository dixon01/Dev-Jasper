// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolDivider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;

    /// <summary>
    /// Container of all the settings for a single transformation.
    /// </summary>
    [Serializable]
    public class SymbolDivider : TransformationConfig
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets the XML element called Symbol.
        /// </summary>
        public string Symbol { get; set; }
        #endregion PROPERTIES
    }
}
