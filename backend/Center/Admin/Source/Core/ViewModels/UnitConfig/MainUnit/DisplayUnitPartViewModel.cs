// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayUnitPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayUnitPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Init
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.MainUnit;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The view model for the load data part.
    /// </summary>
    public class DisplayUnitPartViewModel : SingleEditorPartViewModelBase<DisplayUnitEditorViewModel>
    {
        private decimal unitIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUnitPartViewModel"/> class.
        /// </summary>
        public DisplayUnitPartViewModel()
            : base(new DisplayUnitEditorViewModel())
        {
        }

        /// <summary>
        /// Gets or sets the unit index.
        /// </summary>
        public decimal UnitIndex
        {
            get
            {
                return this.unitIndex;
            }

            set
            {
                this.SetProperty(ref this.unitIndex, value, () => this.UnitIndex);
            }
        }
    }
}
