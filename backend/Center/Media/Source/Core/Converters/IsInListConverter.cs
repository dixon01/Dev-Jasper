// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsInListConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NAME type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The IsInList Converter, checks if a layout element is in a list.
    /// </summary>
    public class IsInListConverter : IMultiValueConverter
    {
        /// <summary>
        /// Checks if a list contains a layout element.
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>
        /// <c>true</c> if the value is within the list; <c>false</c> otherwise.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = false;
            if (values.Length > 1 && values[0] is ObservableCollection<LayoutElementDataViewModelBase>
                && values[1] is GraphicalElementDataViewModelBase)
            {
                var list = values[0] as ObservableCollection<LayoutElementDataViewModelBase>;
                var element = values[1] as GraphicalElementDataViewModelBase;
                result = list.Contains(element);
            }

            return result;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
