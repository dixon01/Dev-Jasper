// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MulitLineTextEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MulitLineTextEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    /// <summary>
    /// The multi-line text editor view model.
    /// </summary>
    public class MulitLineTextEditorViewModel : EditorViewModelBase
    {
        private string text;

        private int minLines = 3;

        private int maxLines = 6;

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
        /// Gets or sets the minimum number of lines.
        /// </summary>
        public int MinLines
        {
            get
            {
                return this.minLines;
            }

            set
            {
                this.SetProperty(ref this.minLines, value, () => this.MinLines);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of lines.
        /// </summary>
        public int MaxLines
        {
            get
            {
                return this.maxLines;
            }

            set
            {
                this.SetProperty(ref this.maxLines, value, () => this.MaxLines);
            }
        }
    }
}
