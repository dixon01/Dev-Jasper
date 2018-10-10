// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsEqualConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsEqualConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The is Equal Converter
    /// </summary>
    public class IsEqualConverter : IMultiValueConverter
    {
        /// <summary>
        /// Compares the values and returns true if equal
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the targetType</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>a value indicating whether the values are equal</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = true;

            for (var i = 1; i < values.Length; i++)
            {
                if (values[0] == null && values[i] == null) { continue; }

                if ((values[0] == null || values[i] == null) || (!values[0].Equals(values[i])))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>a not implemented exception</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
