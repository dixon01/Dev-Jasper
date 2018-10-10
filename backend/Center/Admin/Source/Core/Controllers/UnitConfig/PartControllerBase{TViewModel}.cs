// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartControllerBase{TViewModel}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// Generic base class for all controllers for a unit configuration part.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model this controller controls.
    /// </typeparam>
    public abstract class PartControllerBase<TViewModel> : PartControllerBase
        where TViewModel : PartViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected PartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes this controller and creates the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="PartViewModelBase"/> implementation controlled by this controller.
        /// </returns>
        public override PartViewModelBase Initialize()
        {
            this.ViewModel = this.CreateViewModel();
            this.ViewModel.PropertyChanged += (s, e) => this.RaiseViewModelUpdated(e);
            this.ViewModel.PartKey = this.Key;
            return this.ViewModel;
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="TViewModel"/>.
        /// </returns>
        protected abstract TViewModel CreateViewModel();
    }
}