// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HalfValueConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NAME type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter that takes two values and returns half of the difference.
    /// This can be used to dynamically align controls in the center of a parent.
    /// </summary>
    public class HalfValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converter to get the top or left position for a control that needs to be centered.
        /// </summary>
        /// <param name="values">
        /// value0: total width/height; value1: control width/height
        /// </param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the top or left position of the control</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || !(values[0] is double) || !(values[1] is double))
            {
                return 0;
            }

            var totalWidth = (double)values[0];
            var width = (double)values[1];
            var result = (totalWidth - width) / 2;
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
