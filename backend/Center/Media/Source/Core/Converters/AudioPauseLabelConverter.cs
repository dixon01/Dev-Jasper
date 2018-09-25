// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioPauseLabelConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioPauseLabelConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// The AudioPauseLabelConverter
    /// </summary>
    public class AudioPauseLabelConverter : IValueConverter
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
            var result = value is TimeSpan ? (TimeSpan)value : new TimeSpan();

            return string.Format("{0} {1:hh\\:mm\\:ss\\.f}", MediaStrings.LayoutElement_AudioPauseLabel, result);
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
            throw new NotImplementedException();
        }
    }
}
