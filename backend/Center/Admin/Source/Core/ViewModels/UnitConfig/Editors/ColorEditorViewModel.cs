// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColorEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Windows.Media;

    /// <summary>
    /// The view model for an editor that allows the user to choose a color.
    /// </summary>
    public class ColorEditorViewModel : EditorViewModelBase
    {
        private Color color;

        /// <summary>
        /// Gets or sets the color chosen by the user.
        /// </summary>
        public Color Color
        {
            get
            {
                return this.color;
            }

            set
            {
                this.SetProperty(ref this.color, value, () => this.Color);
                this.MakeDirty();
            }
        }
    }
}
