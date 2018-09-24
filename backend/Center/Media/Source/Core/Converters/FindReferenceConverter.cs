// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindReferenceConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FindReferenceConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;

    /// <summary>
    /// The FindReferenceConverter
    /// </summary>
    public class FindReferenceConverter : IMultiValueConverter
    {
        /// <summary>
        /// the Find Reference Converter
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var needle = values[0];
                if (needle == null || needle == DependencyProperty.UnsetValue)
                {
                    return null;
                }

                var haystack = (IEnumerable)values[1];

                return
                    haystack.OfType<CycleRefConfigDataViewModelBase>()
                            .FirstOrDefault(element => element.Reference == needle);
            }
            catch (Exception)
            {
                return values[0];
            }
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
            try
            {
                return new[]
                       {
                           ((CycleRefConfigDataViewModelBase)value).Reference,
                           Binding.DoNothing
                       };
            }
            catch (Exception)
            {
                return new[] { Binding.DoNothing, Binding.DoNothing };
            }
        }
    }
}
