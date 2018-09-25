// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerGioomPortViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerGioomPortViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using System.Globalization;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// View model that represents a GIOoM port with values of type <see cref="IntegerValues"/>.
    /// </summary>
    public class IntegerGioomPortViewModel : GioomPortViewModelBase
    {
        private int minValue;
        private int maxValue;

        /// <summary>
        /// Gets or sets the minimum value of the port.
        /// </summary>
        public int MinValue
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
        /// Gets or sets the maximum value of the port.
        /// </summary>
        public int MaxValue
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
        /// Gets or sets the integer value of the port.
        /// </summary>
        public int? IntegerValue
        {
            get
            {
                if (this.Value != null)
                {
                    return this.Value.Value;
                }

                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    this.Value = new IOValueViewModel(value.Value.ToString(CultureInfo.InvariantCulture), value.Value);
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        /// <summary>
        /// This method is called whenever <see cref="GioomPortViewModelBase.Value"/> changes.
        /// </summary>
        protected override void HandleValueChanged()
        {
            base.HandleValueChanged();
            this.RaisePropertyChanged(() => this.IntegerValue);
        }
    }
}