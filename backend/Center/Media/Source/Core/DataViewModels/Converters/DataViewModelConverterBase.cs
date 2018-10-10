// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelConverterBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelConverterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Converters
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Base class for converters.
    /// </summary>
    public abstract class DataViewModelConverterBase : IDataViewModelConverter
    {
        /// <summary>
        /// Converts the data model to an equivalent data view model.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="dataViewModel">The dataViewModel.</param>
        public abstract void ToDataViewModel(object dataModel, IDataViewModel dataViewModel);

        /// <summary>
        /// Converts the data view model to an equivalent data model.
        /// </summary>
        /// <param name="dataViewModel">The data view model.</param>
        /// <param name="dataModel">The data model.</param>
        public abstract void ToDataModel(IDataViewModel dataViewModel, object dataModel);

        /// <summary>
        /// Creates the data view model.
        /// </summary>
        /// <typeparam name="T">The type of the data view model.</typeparam>
        /// <param name="dataModelType">Type of the data model.</param>
        /// <returns>A new instance of the data view model.</returns>
        protected virtual T CreateDataViewModel<T>(Type dataModelType)
        {
            throw new NotImplementedException("Not yet implemented");
        }

        /// <summary>
        /// Gets the converter for the specified type.
        /// </summary>
        /// <param name="dataViewModelType">Type of the data view model.</param>
        /// <returns>A new instance of the converter for the specified type.</returns>
        protected virtual IDataViewModelConverter GetConverter(Type dataViewModelType)
        {
            var attribute = FindDataViewModelConverterAttribute(dataViewModelType);
            if (attribute == null)
            {
                throw new Exception(
                    string.Format(
                        "Can't find conversion attribute for data view model '{0}'", dataViewModelType.FullName));
            }

            return (IDataViewModelConverter)Activator.CreateInstance(attribute.ConverterType);
        }

        /// <summary>
        /// Creates the data model.
        /// </summary>
        /// <typeparam name="T">The type of the data model.</typeparam>
        /// <param name="dataViewModelType">Type of the data view model.</param>
        /// <returns>A new instance of the data model.</returns>
        protected virtual T CreateDataModel<T>(Type dataViewModelType)
        {
            var attribute = FindDataViewModelConverterAttribute(dataViewModelType);
            if (attribute != null)
            {
                return (T)Activator.CreateInstance(attribute.ConversionType);
            }

            var typeName = dataViewModelType.Name.Replace("DataViewModel", string.Empty);
            var assembly = typeof(InfomediaConfig).Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(
                    string.Format("Type '{0}' for data view model '{1} not found", typeName, dataViewModelType.Name));
            }

            return (T)Activator.CreateInstance(type);
        }

        private static DataViewModelConverterAttribute FindDataViewModelConverterAttribute(Type dataViewModelType)
        {
            var attribute =
                dataViewModelType.GetCustomAttributes(typeof(DataViewModelConverterAttribute), true)
                                 .OfType<DataViewModelConverterAttribute>()
                                 .SingleOrDefault();
            return attribute;
        }
    }
}
