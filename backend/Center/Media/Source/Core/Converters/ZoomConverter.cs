// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ZoomConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The Zoom Converter expects a double as first parameter and a percentage (double) as second
    /// </summary>
    public class ZoomConverter : IMultiValueConverter
    {
        /// <summary>
        /// Multiplies the value with 100 / parameter
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the multiplied value</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2
                && (values[0] is double || values[0] is int)
                && (values[1] is double || values[1] is int))
            {
                return System.Convert.ToDouble(values[0]) * (System.Convert.ToDouble(values[1]) / 100);
            }

            if (values.Length > 3
                && (values[0] is double || values[0] is int)
                && (values[1] is double || values[1] is int)
                && values[2] is int
                && values[3] is int)
            {
                var width = System.Convert.ToDouble(values[0]);
                var zoomFactor = System.Convert.ToDouble(values[1]) / 100.0;

                var dotDiameter = (int)values[2] * 2;
                var dotSpace = (int)values[3];
                var dotWidth = dotDiameter + dotSpace;
                var totalWidth = width * dotWidth;

                return totalWidth * zoomFactor;
            }

            return 0;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>not used</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
