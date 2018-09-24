// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    /// <summary>
    /// The view model for a text editor.
    /// </summary>
    public class TextEditorViewModel : EditorViewModelBase
    {
        private bool isReadOnly;

        private string text;

        private string watermark;

        /// <summary>
        /// Gets or sets a value indicating whether the text editor is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }

            set
            {
                this.SetProperty(ref this.isReadOnly, value, () => this.IsReadOnly);
            }
        }

        /// <summary>
        /// Gets or sets the text entered by the user.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.SetProperty(ref this.text, value, () => this.Text))
                {
                    this.RaiseErrorsChanged("Text");
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets the watermark shown when the user hasn't entered any text.
        /// </summary>
        public string Watermark
        {
            get
            {
                return this.watermark;
            }

            set
            {
                this.SetProperty(ref this.watermark, value, () => this.Watermark);
            }
        }
    }
}
