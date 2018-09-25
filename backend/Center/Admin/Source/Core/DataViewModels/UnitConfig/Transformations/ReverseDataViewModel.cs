// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReverseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReverseDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="Reverse"/>.
    /// </summary>
    public class ReverseDataViewModel : TransformationDataViewModelBase<Reverse>
    {
        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(Reverse config)
        {
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(Reverse config)
        {
        }
    }
}