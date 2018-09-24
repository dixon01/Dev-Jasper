// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullToVisibilityConverter.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NullToVisibilityConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a null value to the defined <see cref="NullVisibility"/> property.
    /// If the value is not null, <see cref="Visibility.Visible"/> is returned.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToVisibilityConverter"/> class.
        /// </summary>
        public NullToVisibilityConverter()
        {
            this.NullVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Gets or sets the visibility for null values.
        /// </summary>
        /// <value>
        /// The visibility for null values.
        /// </value>
        public Visibility NullVisibility { get; set; }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return this.NullVisibility;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
