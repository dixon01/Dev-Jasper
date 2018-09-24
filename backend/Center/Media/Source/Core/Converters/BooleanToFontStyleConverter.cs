// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToFontStyleConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BooleanToFontStyleConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// The BooleanToFontStyleConverter
    /// </summary>
    public class BooleanToFontStyleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value into a FontStyle.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>
        /// FontStyle.Italic if value is set to <c>true</c>; FontStyle.Normal otherwise
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var boolValue = (bool)value;
                if (boolValue)
                {
                    return FontStyles.Italic;
                }
            }

            return FontStyles.Normal;
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
            throw new NotImplementedException();
        }
    }
}
