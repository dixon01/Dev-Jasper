// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolToFluidContentControlStateConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BoolToFluidContentControlStateConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Converter that converts a <see cref="bool"/> to a <see cref="FluidContentControlState"/> and back.
    /// </summary>
    public class BoolToFluidContentControlStateConverter : IValueConverter
    {
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
            if (value is bool)
            {
                return ((bool)value) ? FluidContentControlState.Large : FluidContentControlState.Normal;
            }

            return value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FluidContentControlState)
            {
                return ((FluidContentControlState)value) == FluidContentControlState.Large;
            }

            return value;
        }
    }
}
