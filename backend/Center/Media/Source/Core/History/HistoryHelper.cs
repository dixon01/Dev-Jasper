// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryHelper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Contains shared functionality used by multiply history entries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// The history helper.
    /// </summary>
    public static class HistoryHelper
    {
        /// <summary>
        /// The unset predefined formula references.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        public static void UnsetPredefinedFormulaReferences(
            LayoutConfigDataViewModel layout,
            IMediaApplicationState state)
        {
            var predefinedFormulas = GetPredefinedFormulas(layout);
            predefinedFormulas.DecreaseReferencesCount(state.CurrentProject);
        }

        /// <summary>
        /// The set predefined formula references.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        public static void SetPredefinedFormulaReferences(
            LayoutConfigDataViewModel layout,
            IMediaApplicationState state)
        {
            var predefinedFormulas = GetPredefinedFormulas(layout);
            predefinedFormulas.IncreaseReferencesCount(state.CurrentProject);
        }

        /// <summary>
        /// The unset media references.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public static void UnsetMediaReferences(LayoutConfigDataViewModel layout, ICommandRegistry commandRegistry)
        {
            foreach (var resolution in layout.Resolutions)
            {
                foreach (var element in resolution.Elements)
                {
                    element.UnsetMediaReference(commandRegistry);
                }
            }
        }

        /// <summary>
        /// The set media references.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public static void SetMediaReferences(LayoutConfigDataViewModel layout, ICommandRegistry commandRegistry)
        {
            foreach (var resolution in layout.Resolutions)
            {
                foreach (var element in resolution.Elements)
                {
                    element.SetMediaReference(commandRegistry);
                }
            }
        }

        private static IEnumerable<EvaluationConfigDataViewModel> GetPredefinedFormulas(
            LayoutConfigDataViewModel layoutConfig)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            foreach (var resolution in layoutConfig.Resolutions)
            {
                foreach (var element in resolution.Elements)
                {
                    result.AddRange(element.GetContainedPredefinedFormulas());
                }
            }

            return result;
        }
    }
}
