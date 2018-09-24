// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckableTreeEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckableTreeEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.ComponentModel;

    /// <summary>
    /// The view model for an editor that shows a tree of checkboxes.
    /// </summary>
    public class CheckableTreeEditorViewModel : EditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckableTreeEditorViewModel"/> class.
        /// </summary>
        public CheckableTreeEditorViewModel()
        {
            this.Root = new CheckableTreeNodeViewModel();
            this.Root.PropertyChanged += this.RootOnPropertyChanged;
        }

        /// <summary>
        /// Gets the root node of the tree.
        /// This node is not shown in the editor, but its children are used as the root nodes in the editor.
        /// </summary>
        public CheckableTreeNodeViewModel Root { get; private set; }

        /// <summary>
        /// Clears the <see cref="DirtyViewModelBase.IsDirty"/> flag on this editor and the entire tree.
        /// </summary>
        public override void ClearDirty()
        {
            this.Root.ClearDirty();
            base.ClearDirty();
        }

        private void RootOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty" && this.Root.IsDirty)
            {
                this.MakeDirty();
            }
        }
    }
}