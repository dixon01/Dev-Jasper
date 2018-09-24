// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelectionViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityReferenceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Collections;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The base class for all entity references.
    /// </summary>
    public abstract class ItemSelectionViewModelBase : ViewModelBase
    {
        private bool isRequired;

        /// <summary>
        /// Gets the items available for selection.
        /// </summary>
        public abstract ICollection Items { get; }

        /// <summary>
        /// Gets or sets the selected item from the <see cref="Items"/> collection.
        /// </summary>
        public abstract object SelectedItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this reference is required or whether null is allowed.
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return this.isRequired;
            }

            set
            {
                this.SetProperty(ref this.isRequired, value, () => this.IsRequired);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Convert.ToString(this.SelectedItem);
        }
    }
}
