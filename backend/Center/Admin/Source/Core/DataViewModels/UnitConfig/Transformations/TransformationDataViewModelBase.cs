// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationDataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model base class for all transformations.
    /// </summary>
    public abstract class TransformationDataViewModelBase : ErrorViewModelBase, ICloneable
    {
        private static readonly Dictionary<Type, Type> ConfigToViewModel = new Dictionary<Type, Type>
            {
                { typeof(Capitalize), typeof(CapitalizeDataViewModel) },
                { typeof(ChainRef), typeof(ChainRefDataViewModel) },
                { typeof(Join), typeof(JoinDataViewModel) },
                { typeof(LawoString), typeof(LawoStringDataViewModel) },
                { typeof(RegexDivider), typeof(RegexDividerDataViewModel) },
                { typeof(RegexMapping), typeof(RegexMappingDataViewModel) },
                { typeof(Replace), typeof(ReplaceDataViewModel) },
                { typeof(Reverse), typeof(ReverseDataViewModel) },
                { typeof(StringMapping), typeof(StringMappingDataViewModel) },
                { typeof(SymbolDivider), typeof(SymbolDividerDataViewModel) }
            };

        /// <summary>
        /// Creates the view model for the given transformation.
        /// </summary>
        /// <param name="transformation">
        /// The transformation.
        /// </param>
        /// <returns>
        /// The <see cref="TransformationDataViewModelBase"/> subclass.
        /// </returns>
        public static TransformationDataViewModelBase Create(TransformationConfig transformation)
        {
            var viewModelType = ConfigToViewModel[transformation.GetType()];
            var viewModel = (TransformationDataViewModelBase)Activator.CreateInstance(viewModelType);
            viewModel.LoadFrom(transformation);
            return viewModel;
        }

        /// <summary>
        /// Gets the list of all supported transformation types (subclasses of <see cref="TransformationConfig"/>).
        /// </summary>
        /// <returns>
        /// The list of <see cref="Type"/>s.
        /// </returns>
        public static Type[] GetSupportedTransformations()
        {
            return ConfigToViewModel.Keys.ToArray();
        }

        /// <summary>
        /// Creates the <see cref="TransformationConfig"/> for this view model.
        /// </summary>
        /// <returns>
        /// The <see cref="TransformationConfig"/>.
        /// </returns>
        public abstract TransformationConfig CreateConfig();

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return Create(this.CreateConfig());
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected abstract void LoadFrom(TransformationConfig config);
    }
}
