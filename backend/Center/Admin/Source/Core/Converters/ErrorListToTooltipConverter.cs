// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorListToTooltipConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorListToTooltipConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;

    /// <summary>
    /// Converter that takes an <see cref="ICollection{T}"/> of <see cref="ErrorItem"/>
    /// and converts it to a string to be used as a tooltip.
    /// </summary>
    public class ErrorListToTooltipConverter : IValueConverter
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
            var errors = value as ICollection<ErrorItem>;
            if (errors == null || errors.Count == 0)
            {
                return null;
            }

            var message = new StringBuilder();
            foreach (var error in errors)
            {
                if (message.Length > 0)
                {
                    message.AppendLine();
                }

                if (errors.Count > 1)
                {
                    // create "bullet points" only when there is more than one error
                    message.Append("- ");
                }

                message.Append(error.Message);
            }

            return message.ToString();
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
            throw new NotSupportedException();
        }
    }
}
