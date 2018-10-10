// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleSourceConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ToggleSourceConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// The ToggleSourceConverter
    /// </summary>
    public class ToggleSourceConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the result on a true value
        /// </summary>
        public object On { get; set; }

        /// <summary>
        /// Gets or sets the result on a false value
        /// </summary>
        public object Off { get; set; }

        /// <summary>
        /// returns one of the two values on or off depending on the given value (boolean)
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? this.On : this.Off;
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
