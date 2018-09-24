// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsDateEvalConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsDateTimeConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Eval;

    /// <summary>
    /// The IsDateTimeConverter
    /// </summary>
    public class IsDateEvalConverter : IValueConverter
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
            var reflectionWrapper = value as ReflectionWrapper;
            if (reflectionWrapper == null)
            {
                return false;
            }

            return reflectionWrapper.SourceType == typeof(DateEvalDataViewModel);
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
