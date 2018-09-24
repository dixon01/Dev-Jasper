// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColumnDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.MasterDetails
{
    /// <summary>
    /// Defines the properties of a data column
    /// </summary>
    public class ColumnDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDescriptor"/> class.
        /// </summary>
        /// <param name="displayIndex">The display index.</param>
        /// <param name="isVisible">if set to <c>true</c> the column should be visible.</param>
        /// <param name="autoWidth">if set to <c>true</c> the width should be automatically set.</param>
        public ColumnDescriptor(int? displayIndex = null, bool isVisible = false, bool autoWidth = true)
        {
            this.DisplayIndex = displayIndex;
            this.IsVisible = isVisible;
            this.AutoWidth = autoWidth;
        }

        /// <summary>
        /// Gets the display index.
        /// </summary>
        public int? DisplayIndex { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the column is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if the column is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the column width should be automatically determined.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the column width should be automatically determined; otherwise, <c>false</c>.
        /// </value>
        public bool AutoWidth { get; private set; }
    }
}