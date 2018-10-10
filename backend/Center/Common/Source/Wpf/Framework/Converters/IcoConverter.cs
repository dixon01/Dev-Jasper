// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IcoConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IcoConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// The icon converter.
    /// </summary>
    public class IcoConverter : IValueConverter
    {
        /// <summary>
        /// The convert.
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
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapFrame result = null;
            Uri bitmapUri = null;

            if (value is string)
            {
                var source = "pack://application:,,," + value;
                bitmapUri = new Uri(source);
            }

            if (value is BitmapImage)
            {
                bitmapUri = ((BitmapImage)value).UriSource;
            }

            if (bitmapUri != null)
            {
                var size = parameter is string ? System.Convert.ToInt32(parameter) : 16;

                var decoder = BitmapDecoder.Create(
                    bitmapUri,
                    BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.OnDemand);

                result = decoder.Frames.SingleOrDefault(f => (int)f.Width == size);
                if (result == default(BitmapFrame))
                {
                    result = decoder.Frames.OrderBy(f => f.Width).First();
                }
            }

            return result;
        }

        /// <summary>
        /// The convert back.
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
        /// The <see cref="object"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}