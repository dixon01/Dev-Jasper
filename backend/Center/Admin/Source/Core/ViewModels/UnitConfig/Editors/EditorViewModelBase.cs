// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditorViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    /// <summary>
    /// The base view model for all editors.
    /// </summary>
    public abstract class EditorViewModelBase : DataErrorViewModelBase
    {
        private string label;

        private bool isEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorViewModelBase"/> class.
        /// </summary>
        public EditorViewModelBase()
        {
            this.IsEnabled = true;
        }

        /// <summary>
        /// Gets or sets the label shown next to the selection.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }

            set
            {
                this.SetProperty(ref this.label, value, () => this.Label);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this editor is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.SetProperty(ref this.isEnabled, value, () => this.IsEnabled);
            }
        }
    }
}