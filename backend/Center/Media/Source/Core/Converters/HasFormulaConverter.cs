// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasFormulaConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HasFormulaConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Eval;

    /// <summary>
    /// The HasFormulaConverter
    /// </summary>
    public class HasFormulaConverter : IValueConverter
    {
        /// <summary>
        /// Tries to convert the <paramref name="value"/> into a <see cref="EvalDataViewModelBase"/> object and
        /// returns <c>true</c> if the value could be converted.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>
        /// <c>true</c> if the value could be converted into a <see cref="EvalDataViewModelBase"/>
        /// ; otherwise <c>false</c>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dynamicDataValue = value as EvalDataViewModelBase;
            if (dynamicDataValue == null)
            {
                return false;
            }

            return true;
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
