// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    /// <summary>
    /// The view model for a number editor (numeric up and down).
    /// </summary>
    public class NumberEditorViewModel : EditorViewModelBase
    {
        private decimal value;

        private decimal minValue;

        private decimal maxValue;

        private bool isInteger;

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public decimal Value
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

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public decimal MinValue
        {
            get
            {
                return this.minValue;
            }

            set
            {
                this.SetProperty(ref this.minValue, value, () => this.MinValue);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public decimal MaxValue
        {
            get
            {
                return this.maxValue;
            }

            set
            {
                this.SetProperty(ref this.maxValue, value, () => this.MaxValue);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether only integers are allowed.
        /// </summary>
        public bool IsInteger
        {
            get
            {
                return this.isInteger;
            }

            set
            {
                this.SetProperty(ref this.isInteger, value, () => this.IsInteger);
            }
        }
    }
}