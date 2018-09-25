// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsCsvMappingEvalFileNameConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsCsvMappingEvalFileNameConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Eval;

    /// <summary>
    /// Converter to determine if the evaluation object is a <see cref="CsvMappingEvalDataViewModel"/>.
    /// </summary>
    public class IsCsvMappingEvalFileNameConverter : IValueConverter
    {
        /// <summary>
        /// Tries to convert an object into a <see cref="CsvMappingEvalDataViewModel"/>.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns><c>true</c> if the value is a <see cref="CsvMappingEvalDataViewModel"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reflectionWrapper = value as ReflectionWrapper;
            if (reflectionWrapper == null || reflectionWrapper.SourceType != typeof(CsvMappingEvalDataViewModel))
            {
                return false;
            }

            return reflectionWrapper.PropertyName == "FileName";
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
