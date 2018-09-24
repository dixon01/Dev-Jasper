// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexMappingDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegexMappingViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="RegexMapping"/>.
    /// </summary>
    public class RegexMappingDataViewModel : MappingTransformationDataViewModelBase<RegexMapping>
    {
        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(RegexMapping config)
        {
            this.LoadMappings(config.Mappings);
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(RegexMapping config)
        {
            config.Mappings = this.CreateMappings();
        }
    }
}