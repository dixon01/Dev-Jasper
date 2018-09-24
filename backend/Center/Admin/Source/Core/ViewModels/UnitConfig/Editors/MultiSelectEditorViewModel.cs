// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiSelectEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// Editor view model for a list of options that can be checked or not.
    /// </summary>
    public class MultiSelectEditorViewModel : EditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectEditorViewModel"/> class.
        /// </summary>
        public MultiSelectEditorViewModel()
        {
            this.Options = new ObservableItemCollection<CheckableOptionViewModel>();
            this.Options.ItemPropertyChanged += this.OptionsOnItemPropertyChanged;
            this.Options.CollectionChanged += this.OptionsOnCollectionChanged;
        }

        /// <summary>
        /// Gets the options from which the user can select.
        /// </summary>
        public ObservableItemCollection<CheckableOptionViewModel> Options { get; private set; }

        /// <summary>
        /// Gets the checked options count.
        /// </summary>
        public int CheckedOptionsCount
        {
            get
            {
                return this.GetCheckedOptions().Count();
            }
        }

        /// <summary>
        /// Gets the list of checked options.
        /// </summary>
        /// <returns>
        /// The list of options that are checked.
        /// </returns>
        public IEnumerable<CheckableOptionViewModel> GetCheckedOptions()
        {
            return this.Options.Where(o => o.IsChecked);
        }

        /// <summary>
        /// Gets the list of checked values.
        /// </summary>
        /// <returns>
        /// The list of the values of the options that are checked.
        /// </returns>
        public IEnumerable<object> GetCheckedValues()
        {
            return this.GetCheckedOptions().Select(o => o.Value);
        }

        private void OptionsOnItemPropertyChanged(
            object sender, ItemPropertyChangedEventArgs<CheckableOptionViewModel> e)
        {
            if (e.PropertyName == "IsChecked")
            {
                this.RaisePropertyChanged(() => this.CheckedOptionsCount);
                this.MakeDirty();
            }
        }

        private void OptionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.CheckedOptionsCount);
        }
    }
}
