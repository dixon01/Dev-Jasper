// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionModeToTextConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionModeToTextConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    /// <summary>
    /// Converter that takes a <see cref="ConnectionMode"/> and returns a translated text.
    /// </summary>
    public class ConnectionModeToTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <returns>
        /// A converted value. On error the method returns an empty string.
        /// </returns>
        /// <param name="value">The enum value.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectionMode enumValue;
            try
            {
                enumValue = (ConnectionMode)Enum.Parse(typeof(ConnectionMode), value.ToString());
            }
            catch (Exception)
            {
                return string.Empty;
            }

            // we will not use enum.toString because Dotfuscate would break it.
            switch (enumValue)
            {
                case ConnectionMode.NotAvailable:
                    return DiagStrings.ConnectionMode_NotAvailable;
                case ConnectionMode.Local:
                    return DiagStrings.ConnectionMode_Local;
                case ConnectionMode.BackgroundSystem:
                    return DiagStrings.ConnectionMode_BackgroundSystem;
                default:
                    return string.Empty;
            }
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
            throw new NotSupportedException();
        }
    }
}
