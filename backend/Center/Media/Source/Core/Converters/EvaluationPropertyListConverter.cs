// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationPropertyListConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationPropertyListConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The EvaluationPropertyListConverter
    /// </summary>
    public class EvaluationPropertyListConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the Help text prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets the list of properties from the given type
        /// and aggregates them into a List of EvaluationPropertyDataViewModel
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new List<EvaluationPropertyDataViewModel>();

            if (value != null && !(value is EvaluationConfigDataViewModel))
            {
                var rm = Resources.MediaStrings.ResourceManager;

                var query = from p in value.GetType().GetProperties()
                            where Attribute.GetCustomAttribute(p, typeof(UserVisiblePropertyAttribute)) != null
                            select new EvaluationPropertyDataViewModel
                                   {
                                       Name = rm.GetString(this.Prefix + p.Name) ?? p.Name,
                                       Evaluation = new ReflectionWrapper(value, p),
                                   };

                result.AddRange(query);
            }

            return result;
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
