// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplaceDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReplaceViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="Replace"/>.
    /// </summary>
    public class ReplaceDataViewModel : MappingTransformationDataViewModelBase<Replace>
    {
        private bool caseSensitive;

        /// <summary>
        /// Gets or sets a value indicating whether case sensitive.
        /// </summary>
        public bool CaseSensitive
        {
            get
            {
                return this.caseSensitive;
            }

            set
            {
                this.SetProperty(ref this.caseSensitive, value, () => this.CaseSensitive);
            }
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(Replace config)
        {
            this.LoadMappings(config.Mappings);
            this.CaseSensitive = config.CaseSensitive;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(Replace config)
        {
            config.Mappings = this.CreateMappings();
            config.CaseSensitive = this.CaseSensitive;
        }
    }
}