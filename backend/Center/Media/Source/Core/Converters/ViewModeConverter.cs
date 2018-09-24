// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModeConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Converter for the <see cref="ViewModeType"/>.
    /// </summary>
    public class ViewModeConverter : IValueConverter
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
            if (value is ViewModeType && parameter is ViewModeType)
            {
                var enumValue = (ViewModeType)parameter;
                var selected = (ViewModeType)value;

                return enumValue == selected;
            }

            return false;
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
            if (value is bool && parameter is ViewModeType)
            {
                var isselected = (bool)value;
                var enumValue = (ViewModeType)parameter;

                if (isselected)
                {
                    return enumValue;
                }
            }

            return Binding.DoNothing;
        }
    }
}
