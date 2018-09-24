// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionGroupViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Base class for option group view models.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.Options
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// Base class for option group view models.
    /// </summary>
    public abstract class OptionGroupViewModelBase : ViewModelBase
    {
        private string label;

        /// <summary>
        /// Gets or sets the group label.
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
        /// Gets or sets the group label tooltip.
        /// </summary>
        public string GroupLabelTooltip { get; set; }

        /// <summary>
        /// Creates a model from this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionGroupBase"/> model.
        /// </returns>
        public abstract OptionGroupBase CreateModel();
    }
}
