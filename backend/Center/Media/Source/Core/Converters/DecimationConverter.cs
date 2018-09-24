// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimationConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DecimationConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter for the Text in the Value Text Box.
    /// Makes sure that the correct decimation is displayed
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DecimationConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target Type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataValue = (double)value;

            return dataValue.ToString("F" + ((uint)parameter));
        }

        /// <summary>
        /// Converts the value back to a double
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target Type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the double</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble(value);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}