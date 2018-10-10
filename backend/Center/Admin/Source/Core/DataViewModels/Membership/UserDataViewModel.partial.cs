// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Membership
{
    /// <summary>
    /// The partial extension of the user data view model.
    /// </summary>
    public partial class UserDataViewModel
    {
        private ItemSelectionViewModelBase cultureSelection;

        private ItemSelectionViewModelBase timeZoneSelection;

        /// <summary>
        /// Gets the culture selection.
        /// This allows to select the culture from a list of valid cultures.
        /// </summary>
        public ItemSelectionViewModelBase CultureSelection
        {
            get
            {
                return this.cultureSelection;
            }

            internal set
            {
                this.SetProperty(ref this.cultureSelection, value, () => this.CultureSelection);
            }
        }

        /// <summary>
        /// Gets the time zone selection.
        /// This allows to select the time zone from a list of valid time zones.
        /// </summary>
        public ItemSelectionViewModelBase TimeZoneSelection
        {
            get
            {
                return this.timeZoneSelection;
            }

            internal set
            {
                this.SetProperty(ref this.timeZoneSelection, value, () => this.TimeZoneSelection);
            }
        }
    }
}
