// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceOptionGroupViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.Options
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Model.Options;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Media.Core.Models.Options;

    /// <summary>
    /// The local resource option group view model.
    /// </summary>
    public class LocalResourceOptionGroupViewModel : OptionGroupViewModelBase
    {
        private TimeSpan removeLocalResourceAfter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalResourceOptionGroupViewModel"/> class.
        /// </summary>
        public LocalResourceOptionGroupViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalResourceOptionGroupViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public LocalResourceOptionGroupViewModel(LocalResourceOptionGroup model)
        {
            this.RemoveLocalResourceAfter = model == null ? new TimeSpan(30, 0, 0, 0) : model.RemoveLocalResourcesAfter;
        }

        /// <summary>
        /// Gets the max days that can be set.
        /// </summary>
        public int MaxDays
        {
            get
            {
                return TimeSpan.MaxValue.Days - 1;
            }
        }

        /// <summary>
        /// Gets or sets the remove local resource after.
        /// </summary>
        public TimeSpan RemoveLocalResourceAfter
        {
            get
            {
                return this.removeLocalResourceAfter;
            }

            set
            {
                this.SetProperty(ref this.removeLocalResourceAfter, value, () => this.RemoveLocalResourceAfter);
            }
        }

        /// <summary>
        /// The create model.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionGroupBase"/>.
        /// </returns>
        public override OptionGroupBase CreateModel()
        {
            return new LocalResourceOptionGroup
                       {
                           RemoveLocalResourcesAfter = this.RemoveLocalResourceAfter
                       };
        }
    }
}
