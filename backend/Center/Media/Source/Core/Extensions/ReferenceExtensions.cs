// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ReferenceExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The ReferenceExtensions.
    /// </summary>
    public static class ReferenceExtensions
    {
        /// <summary>
        /// returns all the contained predefined formulas in the given object.
        /// Reflects over its properties and searches for IDynamicDataValues, then does a deep search over
        /// its formula
        /// </summary>
        /// <param name="obj">the object to be searched</param>
        /// <returns>a list of predefined formulas</returns>
        public static IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas(this object obj)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            var item = obj as EvaluationConfigDataViewModel;
            if (item != null)
            {
                result.Add(item);
            }
            else
            {
                var properties = obj.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (typeof(IDynamicDataValue).IsAssignableFrom(property.PropertyType))
                    {
                        var dynamicDataValue = property.GetValue(obj, new object[0]) as IDynamicDataValue;
                        if (dynamicDataValue != null)
                        {
                            var eval = dynamicDataValue.Formula as EvalDataViewModelBase;
                            if (eval != null)
                            {
                                result.AddRange(eval.GetContainedPredefinedFormulas());
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Reset the contained predefined formulas recursively
        /// </summary>
        /// <param name="obj">the object that potentially contains predefined formulas</param>
        /// <param name="evaluations">the list of original predefined formulas</param>
        /// <param name="decreaseReferenceCountBeforeSet">decreases reference count before set</param>
        /// <returns>the predefined formula</returns>
        public static EvaluationEvalDataViewModel ResetContainedPredefinedFormulaReferences(
            this object obj,
            IList<EvaluationConfigDataViewModel> evaluations,
            bool decreaseReferenceCountBeforeSet = true)
        {
            var item = obj as EvaluationEvalDataViewModel;
            if (item != null)
            {
                return item;
            }

            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (typeof(IDynamicDataValue).IsAssignableFrom(property.PropertyType))
                {
                    ResetDynamicDataValuePredefinedFormula(obj, evaluations, decreaseReferenceCountBeforeSet, property);

                    continue;
                }

                if (typeof(EvalDataViewModelBase).IsAssignableFrom(property.PropertyType))
                {
                    ResetEvalDataPredefinedFormula(obj, evaluations, decreaseReferenceCountBeforeSet, property);

                    continue;
                }

                if (typeof(ExtendedObservableCollection<EvalDataViewModelBase>).IsAssignableFrom(property.PropertyType))
                {
                    if (!ResetEvalListPredefinedFormulas(obj, evaluations, decreaseReferenceCountBeforeSet, property))
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// increase the ReferencesCount of each predefined formula by one
        /// </summary>
        /// <param name="predefinedFormulas">the list of formulas to increase</param>
        /// <param name="project">the project</param>
        public static void IncreaseReferencesCount(
            this IEnumerable<EvaluationConfigDataViewModel> predefinedFormulas,
            MediaProjectDataViewModel project)
        {
            foreach (var clonedPredefinedFormula in predefinedFormulas)
            {
                var formulaName = clonedPredefinedFormula.Name.Value;
                var predefinedFormula =
                    project.InfomediaConfig.Evaluations.FirstOrDefault(pf => pf.Name.Value == formulaName);
                if (predefinedFormula != null)
                {
                    predefinedFormula.ReferencesCount++;
                }
            }
        }

        /// <summary>
        /// decrease the ReferencesCount of each predefined formula by one
        /// </summary>
        /// <param name="predefinedFormulas">the list of formulas to decrease</param>
        /// <param name="project">the project</param>
        public static void DecreaseReferencesCount(
            this IEnumerable<EvaluationConfigDataViewModel> predefinedFormulas,
            MediaProjectDataViewModel project)
        {
            foreach (var clonedPredefinedFormula in predefinedFormulas)
            {
                var formulaName = clonedPredefinedFormula.Name.Value;
                var predefinedFormula =
                    project.InfomediaConfig.Evaluations.FirstOrDefault(pf => pf.Name.Value == formulaName);
                if (predefinedFormula != null)
                {
                    predefinedFormula.ReferencesCount--;
                }
            }
        }

        /// <summary>
        /// Unsets the media references used in a section.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public static void UnsetMediaReferences(
            this SectionConfigDataViewModelBase section, ICommandRegistry commandRegistry)
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var imageSection = section as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  PreviousSelectedResourceHash =
                                                      imageSection.Image == null
                                                          ? null
                                                          : imageSection.Image.Hash
                                              };
                commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
                resourceManager.ImageSectionManager.UnsetReferences(imageSection);
                return;
            }

            var videoSection = section as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  PreviousSelectedResourceHash =
                                                      videoSection.Video == null
                                                          ? null
                                                          : videoSection.Video.Hash
                                              };
                commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
                resourceManager.VideoSectionManager.UnsetReferences(videoSection);
            }

            var poolSection = section as PoolSectionConfigDataViewModel;
            if (poolSection != null && poolSection.Pool != null)
            {
                poolSection.Pool.ReferencesCount--;
                resourceManager.PoolManager.UnsetReferences(poolSection);
            }
        }

        /// <summary>
        /// Sets the media references used in a section.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public static void SetMediaReferences(
            this SectionConfigDataViewModelBase section, ICommandRegistry commandRegistry)
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var imageSection = section as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  CurrentSelectedResourceHash =
                                                      imageSection.Image == null
                                                          ? null
                                                          : imageSection.Image.Hash
                                              };
                commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
                resourceManager.ImageSectionManager.SetReferences(imageSection);
                return;
            }

            var videoSection = section as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  CurrentSelectedResourceHash =
                                                      videoSection.Video == null
                                                          ? null
                                                          : videoSection.Video.Hash
                                              };
                commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
                resourceManager.VideoSectionManager.SetReferences(videoSection);
            }

            var poolSection = section as PoolSectionConfigDataViewModel;
            if (poolSection != null && poolSection.Pool != null)
            {
                if (poolSection.CheckPoolExists())
                {
                    poolSection.Pool.ReferencesCount++;
                }
                else
                {
                    poolSection.Pool = null;
                }

                resourceManager.PoolManager.UnsetReferences(poolSection);
            }
        }

        private static bool ResetEvalListPredefinedFormulas(
         object obj,
         IList<EvaluationConfigDataViewModel> evaluations,
         bool decreaseReferenceCountBeforeSet,
         PropertyInfo property)
        {
            var formulas = property.GetValue(obj, new object[0]) as ExtendedObservableCollection<EvalDataViewModelBase>;
            if (formulas == null)
            {
                return false;
            }

            var formulaReferences = new ExtendedObservableCollection<EvalDataViewModelBase>();
            foreach (var eval in
                formulas.Select(
                    formula =>
                    ResetContainedPredefinedFormulaReferences(formula, evaluations, decreaseReferenceCountBeforeSet))
                        .Where(eval => eval != null))
            {
                formulaReferences.Add(eval);
            }

            if (formulas.Count == formulaReferences.Count)
            {
                property.SetValue(obj, formulaReferences, new object[0]);
            }

            return true;
        }

        private static void ResetEvalDataPredefinedFormula(
            object obj,
            IList<EvaluationConfigDataViewModel> evaluations,
            bool decreaseReferenceCountBeforeSet,
            PropertyInfo property)
        {
            var eval = property.GetValue(obj, new object[0]) as EvalDataViewModelBase;
            if (eval != null)
            {
                var formula = ResetContainedPredefinedFormulaReferences(
                    eval, evaluations, decreaseReferenceCountBeforeSet);
                if (formula != null)
                {
                    eval = formula;
                    property.SetValue(obj, eval, new object[0]);
                }
            }
        }

        private static void ResetDynamicDataValuePredefinedFormula(
            object obj,
            IList<EvaluationConfigDataViewModel> evaluations,
            bool decreaseReferenceCountBeforeSet,
            PropertyInfo property)
        {
            var dynamicDataValue = property.GetValue(obj, new object[0]) as IDynamicDataValue;
            if (dynamicDataValue != null)
            {
                var eval = dynamicDataValue.Formula as EvalDataViewModelBase;
                if (eval != null)
                {
                    var formula = ResetContainedPredefinedFormulaReferences(
                        eval, evaluations, decreaseReferenceCountBeforeSet);
                    if (formula != null)
                    {
                        if (decreaseReferenceCountBeforeSet && formula.Reference != null)
                        {
                            formula.Reference.ReferencesCount--;
                        }

                        dynamicDataValue.Formula = formula;
                        property.SetValue(obj, dynamicDataValue, new object[0]);
                    }
                }
            }
        }
    }
}