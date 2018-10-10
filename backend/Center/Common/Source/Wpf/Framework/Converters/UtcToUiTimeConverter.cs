// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtcToUiTimeConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UtcToUiTimeConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter to change a UTC time to the kind for the UI.
    /// </summary>
    public class UtcToUiTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a UTC time to the kind for the UI.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The local <see cref="DateTime"/> or just the value if it is not a <see cref="DateTime"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                var dateValue = (DateTime)value;
                if (dateValue.Kind == DateTimeKind.Utc)
                {
                    return dateValue.ToLocalTime();
                }

                if (dateValue.Kind == DateTimeKind.Unspecified)
                {
                    var utcDate = DateTime.SpecifyKind(dateValue, DateTimeKind.Utc);
                    return utcDate.ToLocalTime();
                }
            }

            return value;
        }

        /// <summary>
        /// Converts a local time back to UTC.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The UTC <see cref="DateTime"/> or just the value if it is not a <see cref="DateTime"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                var dateValue = (DateTime)value;
                if (dateValue.Kind == DateTimeKind.Local)
                {
                    return dateValue.ToUniversalTime();
                }

                if (dateValue.Kind == DateTimeKind.Unspecified)
                {
                    var localDate = DateTime.SpecifyKind(dateValue, DateTimeKind.Local);
                    return localDate.ToUniversalTime();
                }
            }

            return value;
        }
    }
}
