// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValueToValueConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataValueToValueConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels;

    /// <summary>
    /// The DataValueToValueConverter
    /// </summary>
    public class DataValueToValueConverter : IValueConverter
    {
        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataValue = value as IDataValue;
            if (dataValue != null)
            {
                return dataValue.ValueObject;
            }

            return value;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Activator.CreateInstance(targetType, new[] { value });
        }
    }
}
