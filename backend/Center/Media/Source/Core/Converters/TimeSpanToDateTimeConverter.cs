// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanToDateTimeConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSpanToDateTimeConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.Properties;

    /// <summary>
    /// The BooleanToVisibilityConverter
    /// </summary>
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan)value;
            var timeString = timeSpan.ToString(Settings.Default.TimeEvalFormat, CultureInfo.InvariantCulture);

            DateTime dateTime;
            if (!DateTime.TryParseExact(
                timeString,
               Settings.Default.TimeEvalConvertTimeSpanFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime))
            {
                return null;
            }

            return dateTime;
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
            var dateTime = (DateTime)value;
            var dateString = dateTime.ToString(
                Settings.Default.TimeEvalConvertTimeSpanFormat,
                CultureInfo.InvariantCulture);

            TimeSpan timeSpan;
            if (!TimeSpan.TryParseExact(
                    dateString,
                    Settings.Default.TimeEvalFormat,
                    CultureInfo.InvariantCulture,
                    TimeSpanStyles.None,
                    out timeSpan))
            {
                return null;
            }

            return timeSpan;
        }
    }
}
