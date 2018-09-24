// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanExecuteToBoolConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CanExecuteToBoolConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// Accesses t he can execute.
    /// </summary>
    public class CanExecuteToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Grants access to the value of CanExecute
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var command = value as ICommand;
            if (command != null)
            {
                return command.CanExecute(parameter);
            }

            return false;
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
            throw new NotImplementedException("No back conversion.");
        }
    }
}
