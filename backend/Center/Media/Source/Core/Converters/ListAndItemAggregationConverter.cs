// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListAndItemAggregationConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListAndItemAggregationConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The ListAndItemAggregationConverter
    /// </summary>
    public class ListAndItemAggregationConverter : IMultiValueConverter
    {
        /// <summary>
        /// Aggregates two values (assumed to be list and item) into an aggregated object
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new ListAndItemAggregation
                   {
                       List = values[0] as IList,
                       Item = values[1],
                   };
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

        /// <summary>
        /// The list and item aggregation
        /// </summary>
        public class ListAndItemAggregation
        {
            /// <summary>
            /// Gets or sets the List
            /// </summary>
            public IList List { get; set; }

            /// <summary>
            /// Gets or sets the Item
            /// </summary>
            public object Item { get; set; }
        }
    }
}
