// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionOptionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectionOptionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model for a selection option in a <see cref="SelectionEditorViewModel"/>.
    /// </summary>
    public class SelectionOptionViewModel : ViewModelBase
    {
        private string label;

        private object value;

        private string tooltip;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionOptionViewModel"/> class.
        /// </summary>
        public SelectionOptionViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionOptionViewModel"/> class.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="tooltip">
        /// The tool tip for this element.
        /// </param>
        public SelectionOptionViewModel(string label, object value, string tooltip = null)
        {
            this.Label = label;
            this.Value = value;
            this.ToolTip = tooltip;
        }

        /// <summary>
        /// Gets or sets the label shown in the selection.
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
        /// Gets or sets the object value of this option.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.SetProperty(ref this.value, value, () => this.Value);
            }
        }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        public string ToolTip
        {
            get
            {
                return this.tooltip;
            }

            set
            {
                this.SetProperty(ref this.tooltip, value, () => this.ToolTip);
            }
        }
    }
}