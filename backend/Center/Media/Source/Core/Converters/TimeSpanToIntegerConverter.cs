// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanToIntegerConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSpanToIntegerConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The TimeSpanToIntegerConverter
    /// </summary>
    public class TimeSpanToIntegerConverter : IValueConverter
    {
        /// <summary>
        /// convert timespan to int
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (value is TimeSpan)
            {
                if (parameter.ToString() == "Milliseconds")
                {
                    result = ((TimeSpan)value).TotalMilliseconds;
                }
                else if (parameter.ToString() == "Seconds")
                {
                    result = ((TimeSpan)value).TotalSeconds;
                }
                else if (parameter.ToString() == "Minutes")
                {
                    result = ((TimeSpan)value).TotalMinutes;
                }
                else if (parameter.ToString() == "Hours")
                {
                    result = ((TimeSpan)value).TotalHours;
                }
                else if (parameter.ToString() == "Days")
                {
                    result = ((TimeSpan)value).TotalDays;
                }
                else
                {
                    result = ((TimeSpan)value).TotalSeconds;
                }
            }

            return result;
        }

        /// <summary>
        /// convert int to timespan
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (value is string)
            {
                double number;
                if (double.TryParse(value.ToString(), out number))
                {
                    value = number;
                }
            }

            if (value is double)
            {
                if (parameter.ToString() == "Milliseconds")
                {
                    result = TimeSpan.FromMilliseconds((double)value);
                }
                else if (parameter.ToString() == "Seconds")
                {
                    result = TimeSpan.FromSeconds((double)value);
                }
                else if (parameter.ToString() == "Minutes")
                {
                    result = TimeSpan.FromMinutes((double)value);
                }
                else if (parameter.ToString() == "Hours")
                {
                    result = TimeSpan.FromHours((double)value);
                }
                else if (parameter.ToString() == "Days")
                {
                    result = TimeSpan.FromDays((double)value);
                }
                else
                {
                    result = TimeSpan.FromSeconds((double)value);
                }
            }

            return result;
        }
    }
}
