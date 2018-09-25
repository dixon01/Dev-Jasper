// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuery.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IQuery type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Filters
{
    /// <summary>
    ///  Base class for queries.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Gets or sets the number of items to skip.
        /// </summary>
        int Skip { get; set; }

        /// <summary>
        /// Gets or sets the optional number of items to take.
        /// </summary>
        /// <value>The number of items to take or <c>null</c> to take all values.</value>
        int? Take { get; set; }
    }
}