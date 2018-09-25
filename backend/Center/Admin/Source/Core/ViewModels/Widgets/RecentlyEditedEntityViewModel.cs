// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentlyEditedEntityViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RecentlyEditedEntityViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Widgets
{
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model representing a recently edited entity.
    /// </summary>
    public class RecentlyEditedEntityViewModel : ViewModelBase
    {
        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyEditedEntityViewModel"/> class.
        /// </summary>
        /// <param name="reference">
        /// The reference model.
        /// </param>
        public RecentlyEditedEntityViewModel(RecentlyEditedEntityReference reference)
        {
            this.Reference = reference;
        }

        /// <summary>
        /// Gets the reference model.
        /// </summary>
        public RecentlyEditedEntityReference Reference { get; private set; }

        /// <summary>
        /// Gets or sets the display name for this entity.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.SetProperty(ref this.displayName, value, () => this.DisplayName);
            }
        }
    }
}