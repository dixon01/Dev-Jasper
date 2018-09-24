// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumToBoolConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumToBoolConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The Enum To Boolean Converter
    /// </summary>
    public class EnumToBoolConverter : IValueConverter
    {
        /// <summary>
        /// returns a boolean if the parameter is the current selected enum value
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Array)
            {
                var paramList = parameter as Array;
                return paramList.Cast<object>().Contains(value);
            }

            return value.Equals(parameter);
        }

        /// <summary>
        /// returns the parameter if the value is true
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
