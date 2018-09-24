// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditableSelectionEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditableSelectionEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// The view model for a text editor combined with a combo box for selection of a value.
    /// The user has the possibility to either choose an existing value or type any value.
    /// </summary>
    public class EditableSelectionEditorViewModel : EditorViewModelBase
    {
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSelectionEditorViewModel"/> class.
        /// </summary>
        public EditableSelectionEditorViewModel()
        {
            this.Options = new ObservableCollection<string>();
        }

        /// <summary>
        /// Gets the list of predefined options.
        /// </summary>
        public ObservableCollection<string> Options { get; private set; }

        /// <summary>
        /// Gets or sets the text value.
        /// This can be either a value from <see cref="Options"/> or a value typed in by the user.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.SetProperty(ref this.value, value, () => this.Value))
                {
                    this.MakeDirty();
                }
            }
        }
    }
}
