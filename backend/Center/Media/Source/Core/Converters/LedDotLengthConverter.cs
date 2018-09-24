// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedDotLengthConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NAME type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The NAME
    /// </summary>
    public class LedDotLengthConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a length in pixel to a length in led dots
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3)
            {
                return default(double);
            }

            if (values[0] is int && values[1] is int && values[2] is int)
            {
                var pixellength = (int)values[0];
                var ledDotRadius = (int)values[1];
                var ledDotSpace = (int)values[2];
                var diameter = (ledDotRadius * 2) + ledDotSpace;
                var lengthInLedDots = (double)(pixellength * diameter);
                lengthInLedDots -= ledDotSpace;
                return lengthInLedDots;
            }

            return default(double);
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
