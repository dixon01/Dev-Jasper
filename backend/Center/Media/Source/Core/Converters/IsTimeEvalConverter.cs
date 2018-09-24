// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsTimeEvalConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsTimeEvalConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Eval;

    /// <summary>
    /// Checks if type is contained in the reflection wrapper
    /// </summary>
    public class IsTimeEvalConverter : IValueConverter
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
            var reflectionWrapper = value as ReflectionWrapper;
            if (reflectionWrapper == null)
            {
                return false;
            }

            return reflectionWrapper.SourceType == typeof(TimeEvalDataViewModel);
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
