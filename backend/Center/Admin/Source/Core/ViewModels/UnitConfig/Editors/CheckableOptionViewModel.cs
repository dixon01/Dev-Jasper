// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckableOptionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckableOptionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    /// <summary>
    /// The view model for a checkable selection option in a <see cref="MultiSelectEditorViewModel"/>.
    /// </summary>
    public class CheckableOptionViewModel : SelectionOptionViewModel
    {
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckableOptionViewModel"/> class.
        /// </summary>
        public CheckableOptionViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckableOptionViewModel"/> class.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="isChecked">
        /// A flag indicating if this option should be checked (false by default).
        /// </param>
        public CheckableOptionViewModel(string label, object value, bool isChecked = false)
            : base(label, value)
        {
            this.IsChecked = isChecked;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this option is checked.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                this.SetProperty(ref this.isChecked, value, () => this.IsChecked);
            }
        }
    }
}