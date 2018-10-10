// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GaugeViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GaugeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model to display data in using a gauge.
    /// </summary>
    public class GaugeViewModel : ViewModelBase
    {
        private string label;

        private double maximum;

        private double value;

        private int majorTickStep;

        private string tooltip;

        private string unit;

        /// <summary>
        /// Gets or sets the label shown below the gauge.
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
        /// Gets or sets the maximum value of the gauge.
        /// The minimum value is always zero.
        /// </summary>
        public double Maximum
        {
            get
            {
                return this.maximum;
            }

            set
            {
                this.SetProperty(ref this.maximum, value, () => this.Maximum);
            }
        }

        /// <summary>
        /// Gets or sets the major tick step. This needs to be set properly before using the view model.
        /// </summary>
        public int MajorTickStep
        {
            get
            {
                return this.majorTickStep;
            }

            set
            {
                this.SetProperty(ref this.majorTickStep, value, () => this.MajorTickStep);
            }
        }

        /// <summary>
        /// Gets or sets the value which should be between zero and <see cref="Maximum"/>.
        /// </summary>
        public double Value
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
        /// Gets or sets the measurement unit which is shown inside the gauge.
        /// </summary>
        public string Unit
        {
            get
            {
                return this.unit;
            }

            set
            {
                this.SetProperty(ref this.unit, value, () => this.Unit);
            }
        }

        /// <summary>
        /// Gets or sets the tooltip that is shown when hovering over the gauge.
        /// </summary>
        public string Tooltip
        {
            get
            {
                return this.tooltip;
            }

            set
            {
                this.SetProperty(ref this.tooltip, value, () => this.Tooltip);
            }
        }
    }
}
