// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigatorEntityViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NavigatorEntityViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Navigator
{
    /// <summary>
    /// The view model for a single entity type in the navigator.
    /// </summary>
    public class NavigatorEntityViewModel : NavigatorViewModelBase
    {
        private bool isAllowed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorEntityViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public NavigatorEntityViewModel(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether access to this entity is allowed.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                return this.isAllowed;
            }

            set
            {
                this.SetProperty(ref this.isAllowed, value, () => this.IsAllowed);
            }
        }
    }
}