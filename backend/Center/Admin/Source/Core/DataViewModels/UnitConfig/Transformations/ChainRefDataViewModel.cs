// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChainRefDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChainRefDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="ChainRef"/>.
    /// </summary>
    public class ChainRefDataViewModel : TransformationDataViewModelBase<ChainRef>
    {
        private string transfRef;

        /// <summary>
        /// Gets or sets the transformation reference.
        /// </summary>
        public string TransfRef
        {
            get
            {
                return this.transfRef;
            }

            set
            {
                this.SetProperty(ref this.transfRef, value, () => this.TransfRef);
            }
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(ChainRef config)
        {
            this.TransfRef = config.TransfRef;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(ChainRef config)
        {
            config.TransfRef = this.TransfRef;
        }
    }
}