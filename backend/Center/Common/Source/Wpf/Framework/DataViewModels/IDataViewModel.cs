// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    /// <summary>
    /// Defines a data view model item.
    /// </summary>
    public interface IDataViewModel : IDirty
    {
        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this item is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsItemSelected { get; set; }
    }
}