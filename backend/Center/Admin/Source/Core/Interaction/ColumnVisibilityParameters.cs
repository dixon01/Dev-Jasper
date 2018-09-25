// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnVisibilityParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColumnVisibilityParameters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Interaction
{
    using System.Windows;

    /// <summary>
    /// Parameters to the <see cref="CommandCompositionKeys.Entities.FilterColumn"/> command.
    /// </summary>
    public class ColumnVisibilityParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnVisibilityParameters"/> class.
        /// </summary>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        public ColumnVisibilityParameters(string entityName, string columnName)
        {
            this.EntityName = entityName;
            this.ColumnName = columnName;
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName { get; private set; }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Gets or sets the visibility. By default all columns are visible.
        /// The following return values have the given meaning:
        /// <see cref="System.Windows.Visibility.Visible"/>: the column is visible and can be hidden by the user.
        /// <see cref="System.Windows.Visibility.Collapsed"/>: the column is hidden and can be shown by the user.
        /// <see cref="System.Windows.Visibility.Hidden"/>: the column is completely removed and
        /// can't be selected by the user.
        /// </summary>
        public Visibility Visibility { get; set; }
    }
}
