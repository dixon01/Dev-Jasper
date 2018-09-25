// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowCycleRefIconConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShowCycleRefIconConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;

    /// <summary>
    /// The To Type Converter
    /// </summary>
    public class ShowCycleRefIconConverter : IValueConverter
    {
        /// <summary>
        /// True if value is baggier than parameter
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
            var standardCycleRef = value as StandardCycleRefConfigDataViewModel;
            if (standardCycleRef != null)
            {
                return standardCycleRef.Reference.CyclePackageReferences.Count > 1;
            }

            var eventCycleRef = value as EventCycleRefConfigDataViewModel;
            if (eventCycleRef != null)
            {
                return eventCycleRef.Reference.CyclePackageReferences.Count > 1;
            }

            return false;
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
