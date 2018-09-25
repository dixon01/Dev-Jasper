// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingTransformationDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MappingTransformationDataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model base class for all transformations containing mappings.
    /// </summary>
    /// <typeparam name="TConfig">
    /// The type of config this view model represents.
    /// </typeparam>
    public abstract class MappingTransformationDataViewModelBase<TConfig> : TransformationDataViewModelBase<TConfig>
        where TConfig : TransformationConfig, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingTransformationDataViewModelBase{TConfig}"/> class.
        /// </summary>
        protected MappingTransformationDataViewModelBase()
        {
            this.Mappings = new ObservableItemCollection<MappingDataViewModel>();
            this.Mappings.CollectionChanged += (sender, args) => this.RaisePropertyChanged(() => this.Mappings);
            this.Mappings.ItemPropertyChanged += (sender, args) => this.RaisePropertyChanged(() => this.Mappings);
        }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        public ObservableItemCollection<MappingDataViewModel> Mappings { get; private set; }

        /// <summary>
        /// Loads the mappings from the given list.
        /// </summary>
        /// <param name="mappings">
        /// The mappings.
        /// </param>
        protected void LoadMappings(IEnumerable<Mapping> mappings)
        {
            foreach (var mapping in mappings)
            {
                this.Mappings.Add(new MappingDataViewModel { From = mapping.From, To = mapping.To });
            }
        }

        /// <summary>
        /// Creates the mappings from the <see cref="Mappings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/> of mappings.
        /// </returns>
        protected List<Mapping> CreateMappings()
        {
            return this.Mappings.Select(m => new Mapping { From = m.From, To = m.To }).ToList();
        }
    }
}