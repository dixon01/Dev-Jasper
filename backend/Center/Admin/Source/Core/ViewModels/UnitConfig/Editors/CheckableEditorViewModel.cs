// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckableEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckableEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    /// <summary>
    /// The view model for a checkbox editor.
    /// </summary>
    public class CheckableEditorViewModel : EditorViewModelBase
    {
        private bool? isChecked;

        private bool isThreeState;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckableEditorViewModel"/> class.
        /// </summary>
        public CheckableEditorViewModel()
        {
            this.IsChecked = false;
            this.IsThreeState = false;
        }

        /// <summary>
        /// Gets or sets the is checked flag.
        /// If <see cref="IsThreeState"/> is set to true, this can also be null (meaning "intermediate" state).
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (this.SetProperty(ref this.isChecked, value, () => this.IsChecked))
                {
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox should support three states.
        /// </summary>
        public bool IsThreeState
        {
            get
            {
                return this.isThreeState;
            }

            set
            {
                this.SetProperty(ref this.isThreeState, value, () => this.IsThreeState);
            }
        }
    }
}
