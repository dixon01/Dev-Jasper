// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectionEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The view model for a single selection.
    /// </summary>
    public class SelectionEditorViewModel : EditorViewModelBase
    {
        private SelectionOptionViewModel selectedOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionEditorViewModel"/> class.
        /// </summary>
        public SelectionEditorViewModel()
        {
            this.Options = new ObservableCollection<SelectionOptionViewModel>();
        }

        /// <summary>
        /// Gets the options from which the user can select.
        /// </summary>
        public ObservableCollection<SelectionOptionViewModel> Options { get; private set; }

        /// <summary>
        /// Gets or sets the selected option from the <see cref="Options"/>.
        /// </summary>
        public SelectionOptionViewModel SelectedOption
        {
            get
            {
                return this.selectedOption;
            }

            set
            {
                if (this.SetProperty(ref this.selectedOption, value, () => this.SelectedOption))
                {
                    this.MakeDirty();
                    this.RaisePropertyChanged(() => this.SelectedValue);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="SelectionOptionViewModel.Value"/> of the <see cref="SelectedOption"/>.
        /// This returns null if no option was selected.
        /// </summary>
        public object SelectedValue
        {
            get
            {
                if (this.SelectedOption == null)
                {
                    return null;
                }

                return this.SelectedOption.Value;
            }
        }

        /// <summary>
        /// Tries to select the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be selected (never null).
        /// </param>
        /// <returns>
        /// True if the value was found in the <see cref="Options"/> and was set successfully.
        /// </returns>
        public bool SelectValue(object value)
        {
            var option = this.Options.FirstOrDefault(o => Equals(value, o.Value));
            if (option == null)
            {
                return false;
            }

            this.SelectedOption = option;
            return true;
        }
    }
}