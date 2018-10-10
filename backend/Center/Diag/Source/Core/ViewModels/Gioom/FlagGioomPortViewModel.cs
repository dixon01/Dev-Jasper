// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagGioomPortViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FlagGioomPortViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// View model that represents a GIOoM port with values of type <see cref="FlagValues"/>.
    /// </summary>
    public class FlagGioomPortViewModel : GioomPortViewModelBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the port is set (on).
        /// </summary>
        public bool IsSet
        {
            get
            {
                return this.Value.Value != 0;
            }

            set
            {
                this.Value = value ? new IOValueViewModel("true", 1) : new IOValueViewModel("false", 0);
            }
        }

        /// <summary>
        /// This method is called whenever <see cref="GioomPortViewModelBase.Value"/> changes.
        /// </summary>
        protected override void HandleValueChanged()
        {
            base.HandleValueChanged();
            this.RaisePropertyChanged(() => this.IsSet);
        }
    }
}