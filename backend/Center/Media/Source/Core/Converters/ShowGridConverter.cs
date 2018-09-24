// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowGridConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ShowGridConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Value Converter for the Button Show Property.
    /// Converts from <see cref="bool"/> to <see cref="System.Windows.Visibility"/>
    /// </summary>
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class ShowGridConverter : IValueConverter
    {
        /// <summary>
        /// Converts boolean to 1 or 2
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target Type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>1 or 2</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value))
            {
                return 1;
            }
            
            return 2;
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