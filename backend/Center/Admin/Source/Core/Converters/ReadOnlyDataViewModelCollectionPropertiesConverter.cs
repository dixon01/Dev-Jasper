// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyDataViewModelCollectionPropertiesConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadOnlyDataViewModelCollectionPropertiesConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Gorba.Center.Admin.Core.DataViewModels;

    /// <summary>
    /// Converter that converts all collection properties of a <see cref="ReadOnlyDataViewModelBase"/>
    /// to a list of <see cref="EntityCollectionBase"/> that can be bound to an items control.
    /// </summary>
    public class ReadOnlyDataViewModelCollectionPropertiesConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dvm = value as ReadOnlyDataViewModelBase;
            if (dvm == null)
            {
                return null;
            }

            return
                dvm.GetType()
                    .GetProperties()
                    .Where(p => typeof(EntityCollectionBase).IsAssignableFrom(p.PropertyType))
                    .Select(p => p.GetValue(dvm))
                    .Cast<EntityCollectionBase>()
                    .ToList();
        }

        /// <summary>
        /// Converts back a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
