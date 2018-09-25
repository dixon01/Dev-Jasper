// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The <see cref="EvaluationEvalDataViewModel"/>.
    /// </summary>
    public partial class EvaluationEvalDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            if (this.Reference != null)
            {
                return this.Reference.HumanReadable();
            }

            return string.Empty;
        }

        /// <summary>
        /// Searches for all contained predefined formulas.
        /// </summary>
        /// <returns>
        /// The contained predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas()
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (this.Reference != null)
            {
                result.Add(this.Reference);
            }

            return result;
        }

        /// <summary>
        /// The set contained predefined formulas.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (this.Reference != null)
            {
                if (this.Reference.ClonedFrom != 0)
                {
                    var predefinedFormula = this.GetOriginalPredefinedFormula(predefinedFormulas, this.Reference);
                    result.Add(predefinedFormula);
                    this.Reference = predefinedFormula;
                }
                else
                {
                    this.Reference.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            return result;
        }

        private EvaluationConfigDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var eval in applicationState.CurrentProject.InfomediaConfig.Evaluations)
            {
                if (eval.Name.Value == this.ReferenceName)
                {
                    return eval;
                }
            }

            Logger.Trace("Evaluation reference with name {0} not found in Evaluations.", this.ReferenceName);
            return null;
        }
    }
}
