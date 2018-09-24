// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceProgressConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceProgressConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The ResourceProgressConverter
    /// </summary>
    public class ResourceProgressConverter : IMultiValueConverter
    {
        /// <summary>
        /// Returns the progress in percent
        /// </summary>
        /// <param name="values">
        /// the values; value[0]: current resource number, value[1]: previous number, value[2]: total resources
        /// </param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>The progress in percent; 0 if the values are not of type int</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() >= 2 && values[0] is double && values[1] is int)
            {
                var currentResource = (double)values[0];
                var total = (int)values[1];
                if (total > 0)
                {
                    var result = (currentResource * 100) / total;
                    return result;
                }
            }

            return 0d;
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
