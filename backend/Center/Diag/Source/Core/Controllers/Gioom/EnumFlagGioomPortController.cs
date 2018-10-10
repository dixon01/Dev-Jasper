// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFlagGioomPortController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumFlagGioomPortController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Gioom
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Controller for GIOoM ports with values of type <see cref="EnumFlagValues"/>.
    /// </summary>
    public class EnumFlagGioomPortController : GioomPortControllerBase<EnumFlagValues, EnumFlagGioomPortViewModel>
    {
        private bool isUpdating;

        /// <summary>
        /// Prepares the view model with the given possible values.
        /// </summary>
        /// <param name="values">
        /// The possible values of the port.
        /// </param>
        protected override void PrepareViewModel(EnumFlagValues values)
        {
            Array.ForEach(
                values.Values,
                v => this.ViewModel.PossibleValues.Add(new SelectableIOValueViewModel(v.Name, v.Value)));

            this.ViewModel.PossibleValues.ItemPropertyChanged += this.PossibleValuesOnItemPropertyChanged;
        }

        /// <summary>
        /// Updates the view model with the given value.
        /// </summary>
        /// <param name="value">
        /// The new value.
        /// </param>
        protected override void UpdateViewModel(IOValueViewModel value)
        {
            this.isUpdating = true;
            try
            {
                base.UpdateViewModel(value);
                foreach (var item in this.ViewModel.PossibleValues)
                {
                    item.IsSelected = (item.Value & value.Value) == item.Value && (item.Value != 0 || value.Value == 0);
                }
            }
            finally
            {
                this.isUpdating = false;
            }
        }

        /// <summary>
        /// This method is called when one of the <see cref="GioomPortControllerBase.ViewModel"/>'s property changes.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the changed property.
        /// </param>
        protected override void HandleViewModelPropertyChange(string propertyName)
        {
            if (this.isUpdating)
            {
                return;
            }

            base.HandleViewModelPropertyChange(propertyName);
        }

        private void PossibleValuesOnItemPropertyChanged(
            object sender, ItemPropertyChangedEventArgs<SelectableIOValueViewModel> e)
        {
            if (this.isUpdating)
            {
                return;
            }

            var value = this.ViewModel.PossibleValues.Where(v => v.IsSelected)
                .Aggregate(0, (current, item) => current | item.Value);

            this.UpdatePortValue(value);
        }
    }
}