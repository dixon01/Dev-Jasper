// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalElementDataModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    /// <summary>
    /// Defines the base properties of all layout element data models.
    /// </summary>
    public partial class GraphicalElementDataModelBase
    {
        /// <summary>
        /// Gets or sets the group this element belongs to.
        /// </summary>
        public GraphicalElementGroupDataModel Group { get; set; }
    }
}
