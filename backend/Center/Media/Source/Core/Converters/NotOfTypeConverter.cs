// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotOfTypeConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotOfTypeConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The To Type Converter
    /// </summary>
    public class NotOfTypeConverter : IValueConverter
    {
        /// <summary>
        /// Returns the type of the Value
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = parameter as Type;
            if (type == null || value == null)
            {
                return false;
            }

            return value.GetType() != type;
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
            throw new NotImplementedException();
        }
    }
}
