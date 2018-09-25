// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationDataViewModelBase{TConfig}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationDataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The transformation view model base.
    /// </summary>
    /// <typeparam name="TConfig">
    /// The type of config this view model represents.
    /// </typeparam>
    public abstract class TransformationDataViewModelBase<TConfig> : TransformationDataViewModelBase
        where TConfig : TransformationConfig, new()
    {
        /// <summary>
        /// Creates the <see cref="TransformationConfig"/> for this view model.
        /// </summary>
        /// <returns>
        /// The <see cref="TransformationConfig"/>.
        /// </returns>
        public sealed override TransformationConfig CreateConfig()
        {
            var config = new TConfig();
            this.SaveTo(config);
            return config;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return typeof(TConfig).Name;
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected sealed override void LoadFrom(TransformationConfig config)
        {
            this.LoadFrom((TConfig)config);
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected abstract void LoadFrom(TConfig config);

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected abstract void SaveTo(TConfig config);
    }
}