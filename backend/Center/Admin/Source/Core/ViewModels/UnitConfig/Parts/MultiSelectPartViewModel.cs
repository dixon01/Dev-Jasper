// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiSelectPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// A part view model that contains a <see cref="MultiSelectEditorViewModel"/>.
    /// </summary>
    public class MultiSelectPartViewModel : SingleEditorPartViewModelBase<MultiSelectEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectPartViewModel"/> class.
        /// </summary>
        public MultiSelectPartViewModel()
            : base(new MultiSelectEditorViewModel())
        {
            this.Editor.Options.ItemPropertyChanged += this.OptionsOnItemPropertyChanged;
        }

        private void OptionsOnItemPropertyChanged(
            object sender, ItemPropertyChangedEventArgs<CheckableOptionViewModel> e)
        {
            if (e.PropertyName == "IsChecked")
            {
                this.MakeDirty();
            }
        }
    }
}
