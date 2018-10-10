// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvalDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvalDataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Base class for evaluation data view models.
    /// </summary>
    public abstract class EvalDataViewModelBase : FormulaDataViewModelBase, ICloneable
    {
        /// <summary>
        /// Exports the data view model.
        /// </summary>
        /// <param name="parameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        /// <returns>
        /// The exported entity.
        /// </returns>
        public EvalBase Export(object parameters = null)
        {
            var eval = (EvalBase)this.CreateExportObject();
            this.DoExport(eval, parameters);
            return eval;
        }

        /// <summary>
        /// Converts a data view model to a data model.
        /// </summary>
        /// <returns>
        /// The converted <see cref="EvalDataModelBase"/>.
        /// </returns>
        public EvalDataModelBase ToDataModel()
        {
            var model = (EvalDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// creates a human readable string representation
        /// </summary>
        /// <returns>the readable string</returns>
        public virtual string HumanReadable()
        {
            return this.ToString();
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The cloned object.
        /// </returns>
        public abstract object Clone();

        /// <summary>
        /// Searches for all contained predefined formulas.
        /// </summary>
        /// <returns>
        /// The contained predefined formulas.
        /// </returns>
        public abstract IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas();

        /// <summary>
        /// The set contained predefined formulas.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <returns>
        /// List of predefined formulas which are now used
        /// </returns>
        public abstract IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas);

        /// <summary>
        /// Checks if a compatibility conversion for CSV mapping is required
        /// </summary>
        /// <param name="exportParameters">
        /// The export parameters.
        /// </param>
        /// <returns>
        /// True if is required.
        /// </returns>
        protected static bool CsvMappingCompatibilityRequired(object exportParameters)
        {
            var parameters = exportParameters as ExportCompatibilityParameters;
            return parameters != null && parameters.CsvMappingCompatibilityRequired;
        }

        /// <summary>
        /// The lookup predefined formula.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <param name="clone">
        /// The clone.
        /// </param>
        /// <returns>
        /// The <see cref="EvaluationConfigDataViewModel"/>.
        /// </returns>
        protected EvaluationConfigDataViewModel GetOriginalPredefinedFormula(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas, EvalDataViewModelBase clone)
        {
            return predefinedFormulas.FirstOrDefault(pf => pf.GetHashCode() == clone.ClonedFrom);
        }

        /// <summary>
        /// Validates the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The list of error messages for the given properties. Empty enumeration if no error was found.
        /// </returns>
        protected override IEnumerable<string> Validate(string propertyName)
        {
            if (propertyName == "ClonedFrom")
            {
                var result = base.Validate(propertyName).ToList();
                var controller =
                    ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = "=" + this.HumanReadable();
                    var formula = controller.ParseFormula(formulaString);
                    if (formula == null)
                    {
                        result.Add("Formula invalid");
                    }
                }
                catch (Exception e)
                {
                    result.Add(e.Message);
                }

                return result;
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Creates an empty entity object to export.
        /// </summary>
        /// <returns>
        /// The entity.
        /// </returns>
        protected abstract object CreateExportObject();

        /// <summary>
        /// Creates an empty the data model object.
        /// </summary>
        /// <returns>
        /// The data model object.
        /// </returns>
        protected abstract object CreateDataModelObject();

        /// <summary>
        /// Converts a code conversion formula into a csv mapping formula.
        /// </summary>
        /// <param name="codeConversion">
        /// The code conversion.
        /// </param>
        /// <returns>
        /// The <see cref="CsvMappingEval"/>.
        /// </returns>
        protected CsvMappingEval ConvertCodeConversionToCsvMapping(CodeConversionEval codeConversion)
        {
            var csvMapping = new CsvMappingEval
                                 {
                                     FileName = codeConversion.FileName,
                                     DefaultValue =
                                         new DynamicProperty
                                             {
                                                 Evaluation =
                                                     new GenericEval
                                                         {
                                                             Column = 0,
                                                             Language = 0,
                                                             Table = 10,
                                                             Row = 0
                                                         }
                                             }
                                 };
            var match0 = new MatchDynamicProperty
            {
                Column = 0,
                Evaluation = new GenericEval
                {
                    Column = 1,
                    Language = 0,
                    Table = 10,
                    Row = 0
                }
            };
            var match1 = new MatchDynamicProperty
            {
                Column = 1,
                Evaluation =
                    new GenericEval
                    {
                        Column = 0,
                        Language = 0,
                        Table = 10,
                        Row = 0
                    }
            };

            csvMapping.Matches.Add(match0);
            csvMapping.Matches.Add(match1);
            csvMapping.OutputFormat = codeConversion.UseImage ? "{2}" : "{3}";
            return csvMapping;
        }
    }
}
