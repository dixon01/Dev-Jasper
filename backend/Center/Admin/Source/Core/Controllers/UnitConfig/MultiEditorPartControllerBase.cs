// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiEditorPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiEditorPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// Base class for all controllers for a unit configuration part that
    /// use the <see cref="MultiEditorPartViewModel"/>.
    /// </summary>
    public abstract class MultiEditorPartControllerBase : PartControllerBase<MultiEditorPartViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiEditorPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected MultiEditorPartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Initializes this controller and creates the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/> controlled by this controller.
        /// </returns>
        public sealed override PartViewModelBase Initialize()
        {
            base.Initialize();
            this.ViewModel.Editors.ItemPropertyChanged += this.EditorsOnItemPropertyChanged;
            return this.ViewModel;
        }

        private void EditorsOnItemPropertyChanged(object s, ItemPropertyChangedEventArgs<EditorViewModelBase> e)
        {
            this.RaiseViewModelUpdated(e);
        }
    }
}