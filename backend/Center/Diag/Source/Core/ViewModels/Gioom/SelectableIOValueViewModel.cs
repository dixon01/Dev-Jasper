// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectableIOValueViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectableIOValueViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    /// <summary>
    /// Extension of <see cref="IOValueViewModel"/> that can be selected and deselected in the view.
    /// </summary>
    public class SelectableIOValueViewModel : IOValueViewModel
    {
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableIOValueViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The human readable name.
        /// </param>
        /// <param name="value">
        /// The integer value.
        /// </param>
        public SelectableIOValueViewModel(string name, int value)
            : base(name, value)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this value is selected in the view.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.SetProperty(ref this.isSelected, value, () => this.IsSelected);
            }
        }
    }
}