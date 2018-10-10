// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiEditorPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiEditorPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// The view model for a part that contains one or more <see cref="EditorViewModelBase"/>.
    /// </summary>
    public class MultiEditorPartViewModel : PartViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiEditorPartViewModel"/> class.
        /// </summary>
        public MultiEditorPartViewModel()
        {
            this.Editors = new ObservableItemCollection<EditorViewModelBase>();
            this.Editors.ItemPropertyChanged += this.EditorsOnItemPropertyChanged;
            this.Editors.CollectionChanged += this.EditorsOnCollectionChanged;
        }

        /// <summary>
        /// Gets all errors of the <see cref="Editors"/> in this part.
        /// </summary>
        public override ICollection<ErrorItem> Errors
        {
            get
            {
                return
                    this.Editors.SelectMany(
                        editor => editor.GetAllErrors(null),
                        (editor, error) => new ErrorItem(error.State, editor.Label + ": " + error.Message)).ToList();
            }
        }

        /// <summary>
        /// Gets the editors.
        /// </summary>
        public ObservableItemCollection<EditorViewModelBase> Editors { get; private set; }

        /// <summary>
        /// Clears the <see cref="DirtyViewModelBase.IsDirty"/> flag.
        /// The default behavior clears the flag on the current object and all its children.
        /// </summary>
        public override void ClearDirty()
        {
            base.ClearDirty();
            foreach (var editor in this.Editors)
            {
                editor.ClearDirty();
            }
        }

        private void EditorsOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<EditorViewModelBase> e)
        {
            if (e.PropertyName == "IsDirty")
            {
                this.UpdateIsDirty();
            }
        }

        private void EditorsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var remove in e.OldItems.OfType<EditorViewModelBase>())
                {
                    remove.ErrorsChanged -= this.EditorOnErrorsChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var add in e.NewItems.OfType<EditorViewModelBase>())
                {
                    add.ErrorsChanged += this.EditorOnErrorsChanged;
                }
            }

            this.UpdateErrorState();
            this.UpdateIsDirty();
        }

        private void EditorOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.UpdateErrorState();
        }

        private void UpdateIsDirty()
        {
            this.IsDirty = this.Editors.Any(ed => ed.IsDirty);
        }

        private void UpdateErrorState()
        {
            var errorState = ErrorState.Ok;
            foreach (var error in this.Editors.SelectMany(e => e.GetAllErrors(null)))
            {
                if (error.State <= errorState)
                {
                    continue;
                }

                errorState = error.State;
                if (errorState == ErrorState.Error)
                {
                    break;
                }
            }

            this.SetErrorState(errorState);
            this.RaisePropertyChanged(() => this.Errors);
        }
    }
}
