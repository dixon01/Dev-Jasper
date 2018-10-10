// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiSelectPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// Base class for part controllers that use a <see cref="MultiSelectPartViewModel"/>.
    /// </summary>
    public abstract class MultiSelectPartControllerBase : PartControllerBase<MultiSelectPartViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected MultiSelectPartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Initializes this controller and creates the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="PartViewModelBase"/> implementation controlled by this controller.
        /// </returns>
        public sealed override PartViewModelBase Initialize()
        {
            base.Initialize();
            this.ViewModel.Editor.Options.ItemPropertyChanged += this.OptionsOnItemPropertyChanged;
            return this.ViewModel;
        }

        private void OptionsOnItemPropertyChanged(
            object sender,
            ItemPropertyChangedEventArgs<CheckableOptionViewModel> e)
        {
            if (e.PropertyName != "IsChecked")
            {
                return;
            }

            this.RaiseViewModelUpdated(e);
        }
    }
}