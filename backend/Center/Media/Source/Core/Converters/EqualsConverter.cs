// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualsConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EqualsConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The EqualsConverter
    /// </summary>
    public class EqualsConverter : IMultiValueConverter
    {
        /// <summary>
        /// compares the values and returns true if all are equal
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var first = values.FirstOrDefault();
            var result = values.All(value => (value == null && first == null) || (value != null && value.Equals(first)));
            return result;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
