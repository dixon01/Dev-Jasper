﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayStringConverter.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayStringConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Returns the string or a replacement if it is null, empty or white space.
    /// </summary>
    public class DisplayStringConverter : IValueConverter
    {
        private const string DefaultEmptyReplacement = "-";

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayStringConverter"/> class.
        /// </summary>
        public DisplayStringConverter()
        {
            this.EmptyReplacement = DefaultEmptyReplacement;
        }

        /// <summary>
        /// Gets or sets the empty replacement.
        /// </summary>
        /// <value>
        /// The empty replacement.
        /// </value>
        public string EmptyReplacement { get; set; }

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
                return this.EmptyReplacement;
            }

            var s = value as string;
            return string.IsNullOrWhiteSpace(s) ? this.EmptyReplacement : s;
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
