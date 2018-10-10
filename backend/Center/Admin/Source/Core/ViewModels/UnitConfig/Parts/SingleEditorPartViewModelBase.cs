// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleEditorPartViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SingleEditorPartViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Base class for all part view models that contain a single editor.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the editor in this part.
    /// </typeparam>
    public abstract class SingleEditorPartViewModelBase<T> : PartViewModelBase
        where T : DataErrorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleEditorPartViewModelBase{T}"/> class.
        /// </summary>
        /// <param name="editor">
        /// The editor to be used.
        /// </param>
        protected SingleEditorPartViewModelBase(T editor)
        {
            this.Editor = editor;
            this.Editor.PropertyChanged += this.EditorOnPropertyChanged;
            this.Editor.ErrorsChanged += this.EditorOnErrorsChanged;
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        public T Editor { get; private set; }

        /// <summary>
        /// Gets all errors of this part.
        /// </summary>
        public override ICollection<ErrorItem> Errors
        {
            get
            {
                return this.Editor.GetAllErrors(null);
            }
        }

        /// <summary>
        /// Clears the <see cref="DirtyViewModelBase.IsDirty"/> flag and the one of the child.
        /// </summary>
        public override void ClearDirty()
        {
            base.ClearDirty();
            this.Editor.ClearDirty();
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty" && this.Editor.IsDirty)
            {
                this.IsDirty = true;
            }
        }

        private void EditorOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            var errorState = ErrorState.Ok;
            foreach (var error in this.Editor.GetAllErrors(null))
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