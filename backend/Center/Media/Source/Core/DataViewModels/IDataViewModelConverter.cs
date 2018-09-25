// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataViewModelConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDataViewModelConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Defines a converter between data models and equivalent data view models.
    /// </summary>
    public interface IDataViewModelConverter
    {
        /// <summary>
        /// Converts the data model to an equivalent data view model.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="dataViewModel">The dataViewModel.</param>
        void ToDataViewModel(object dataModel, IDataViewModel dataViewModel);

        /// <summary>
        /// Converts the data view model to an equivalent data model.
        /// </summary>
        /// <param name="dataViewModel">The data view model.</param>
        /// <param name="dataModel">The data model.</param>
        void ToDataModel(IDataViewModel dataViewModel, object dataModel);
    }
}