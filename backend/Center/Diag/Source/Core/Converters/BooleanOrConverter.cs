// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanOrConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BooleanOrConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The BooleanOrConverter
    /// </summary>
    public class BooleanOrConverter : IMultiValueConverter
    {
        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="values">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = false;

            if (values.Length > 0)
            {
                result = values.Any(t => (bool)t);
            }

            return result;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
