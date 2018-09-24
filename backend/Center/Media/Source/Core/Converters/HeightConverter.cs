// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeightConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The HeightConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Value Conversion Class for the button height.
    /// Divides the Height of the control by two to get the height for one button.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class HeightConverter : IValueConverter
    {
        /// <summary>
        /// Divides the value with 2
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target Type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the value divided by 2</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 2.0;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target Type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>not implemented</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}