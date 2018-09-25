// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolConfigDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The pool config data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Presentation
{
    using System.Collections.Generic;

    /// <summary>
    /// The pool config data model.
    /// </summary>
    public partial class PoolConfigDataModel
    {
        /// <summary>
        /// Gets or sets the resource references.
        /// </summary>
        /// <value>
        /// The resource references.
        /// </value>
        public List<ResourceReference> ResourceReferences { get; set; }

        /// <summary>
        /// Gets or sets the references count.
        /// </summary>
        public int ReferencesCount { get; set; }
    }
}
