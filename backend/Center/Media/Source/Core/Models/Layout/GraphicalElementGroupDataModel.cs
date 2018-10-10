// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalElementGroupDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a group of <see cref="GraphicalElementDataModelBase"/> items.
    /// </summary>
    public class GraphicalElementGroupDataModel : DataModelBase
    {
        private readonly List<GraphicalElementDataModelBase> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicalElementGroupDataModel"/> class.
        /// </summary>
        public GraphicalElementGroupDataModel()
        {
            this.items = new List<GraphicalElementDataModelBase>();
        }

        /// <summary>
        /// Gets the collection of <see cref="GraphicalElementDataModelBase"/>.
        /// </summary>
        public List<GraphicalElementDataModelBase> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string GroupName { get; set; }
    }
}
