// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorToolbarSelectionConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditorToolbarSelectionConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The EditorToolbarSelectionConverter
    /// </summary>
    public class EditorToolbarSelectionConverter : IValueConverter
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
            if (value is EditorToolType && parameter is EditorToolType)
            {
                var enumValue = (EditorToolType)parameter;
                var selected = (EditorToolType)value;

                return enumValue == selected;
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
            if (value is bool && parameter is EditorToolType)
            {
                var isselected = (bool)value;
                var enumValue = (EditorToolType)parameter;

                if (isselected)
                {
                    return enumValue;
                }
            }

            return Binding.DoNothing;
        }
    }
}
