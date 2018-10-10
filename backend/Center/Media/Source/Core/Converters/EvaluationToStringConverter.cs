// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationToStringConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationToStringConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;

    using Gorba.Center.Common.Wpf.Framework.Converters;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The EvaluationToStringConverter
    /// </summary>
    public class EvaluationToStringConverter : IFormulaConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the unchanged value.
        /// </summary>
        public object UnchangedValue { get; set; }

        /// <summary>
        /// Converts an <see cref="EvalDataViewModelBase"/> to its <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataValue = value as EvalDataViewModelBase;
            if (dataValue == null)
            {
                return null;
            }

            var equalSign = "=";
            if (parameter != null)
            {
                bool boolValue;
                if (bool.TryParse(parameter.ToString(), out boolValue) && !boolValue)
                {
                    equalSign = string.Empty;
                }
            }

            return equalSign + dataValue.HumanReadable();
        }

        /// <summary>
        /// Converts a <see cref="string"/> into an <see cref="EvalDataViewModelBase"/>.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueString = value as string;
            if (valueString == null)
            {
                return null;
            }

            var needsEqualsSign = false;
            if (parameter is bool)
            {
                needsEqualsSign = bool.Parse(parameter.ToString());
            }

            if (valueString.StartsWith("=") || !needsEqualsSign)
            {
                var appController = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                var formulaController = appController.ShellController.FormulaController;
                try
                {
                    var formula = valueString;
                    if (!needsEqualsSign && !valueString.StartsWith("="))
                    {
                        formula = "= " + valueString;
                    }

                    var result = formulaController.ParseFormula(formula);

                    return result;
                }
                catch (Exception e)
                {
                    var message = string.Format("Unable to parse formula string {0}.", valueString);
                    Logger.DebugException(message, e);
                    return this.UnchangedValue;
                }
            }

            return value;
        }
    }
}
