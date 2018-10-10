// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectComboBoxConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiSelectComboBoxConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls.MultiselectCombobox
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The multi select combo box converter.
    /// </summary>
    public class MultiSelectComboBoxConverter : IValueConverter
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
            var displayMemberPath = parameter as string;
            if (string.IsNullOrWhiteSpace(displayMemberPath) || value == null)
            {
                return string.Empty;
            }

            var query = ((IEnumerable<object>)value).Select(
                item =>
                    {
                        var propertyInfo = DataControlHelper.GetPropertyInfo(item.GetType(), displayMemberPath);
                        if (propertyInfo == null)
                        {
                            return string.Empty;
                        }

                        return propertyInfo.GetValue(item, null);
                    }).ToArray();

            return string.Join(", ", query);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target Type.
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