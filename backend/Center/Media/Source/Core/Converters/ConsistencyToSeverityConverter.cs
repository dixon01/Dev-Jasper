// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsistencyToSeverityConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConsistencyConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The ConsistencyConverter
    /// </summary>
    public class ConsistencyToSeverityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a list of <see cref="ConsistencyMessageDataViewModel"/>s to <see cref="Severity"/>.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ExtendedObservableCollection<ConsistencyMessageDataViewModel>;
            if (collection == null)
            {
                return Severity.None;
            }

            if (collection.Any(message => message.Severity == Severity.CompatibilityIssue))
            {
                return Severity.CompatibilityIssue;
            }

            if (collection.Any(message => message.Severity == Severity.Error))
            {
                return Severity.Error;
            }

            if (collection.Any(message => message.Severity == Severity.Warning))
            {
                return Severity.Warning;
            }

            return Severity.None;
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
            throw new NotImplementedException();
        }
    }
}
