// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanToDateTimeConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSpanToDateTimeConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter that can convert a <see cref="TimeSpan"/> to a <see cref="DateTime"/>.
    /// The time span must be between 00:00:00.000 and 23:59:59.999 (i.e. zero or positive and not greater than 1 day).
    /// </summary>
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        private static readonly DateTime DefaultDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (TimeSpan?)value;
            if (targetType == typeof(DateTime))
            {
                return DefaultDate + input.Value;
            }

            if (targetType == typeof(DateTime?))
            {
                return input.HasValue ? DefaultDate + input.Value : (DateTime?)null;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts back a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (DateTime?)value;
            if (targetType == typeof(TimeSpan))
            {
                return input.Value.TimeOfDay;
            }

            if (targetType == typeof(TimeSpan?))
            {
                return input.HasValue ? input.Value.TimeOfDay : (TimeSpan?)null;
            }

            throw new NotSupportedException();
        }
    }
}
