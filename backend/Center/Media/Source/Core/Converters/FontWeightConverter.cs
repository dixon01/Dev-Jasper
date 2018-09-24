// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontWeightConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontWeightConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// The FontWeightConverter
    /// </summary>
    public class FontWeightConverter : IValueConverter
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
            var intValue = value as int?;
            if (intValue == null)
            {
                return null;
            }

            if (intValue < 200)
            {
                return FontWeights.Thin;
            }

            if (intValue < 300)
            {
                return FontWeights.ExtraLight;
            }

            if (intValue < 400)
            {
                return FontWeights.Light;
            }

            if (intValue < 500)
            {
                return FontWeights.Normal;
            }

            if (intValue < 600)
            {
                return FontWeights.Medium;
            }

            if (intValue < 700)
            {
                return FontWeights.DemiBold;
            }

            if (intValue < 800)
            {
                return FontWeights.Bold;
            }

            if (intValue < 900)
            {
                return FontWeights.ExtraBold;
            }

            return FontWeights.Heavy;
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
