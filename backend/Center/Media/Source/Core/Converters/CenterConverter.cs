// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CenterConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The Center Converter, converts the dimension values to the respective coordinates
    /// </summary>
    public class CenterConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the dimension values to the respective coordinates
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the coordinate</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 2 && values[0] is double && values[1] is double && values[2] is double && values[3] is double)
            {
                var canvasDimension = System.Convert.ToDouble(values[0]);
                var controlDimension = System.Convert.ToDouble(values[1]);
                var offset = System.Convert.ToDouble(values[2]);
                var zoom = System.Convert.ToDouble(values[3]) / 100d;
                return ((canvasDimension - (controlDimension * zoom)) / 2) + offset;
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
