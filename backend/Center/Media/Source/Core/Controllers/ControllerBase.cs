// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Defines the base class for controllers.
    /// </summary>
    public abstract class ControllerBase
    {
        [SuppressMessage(
        "StyleCopPlus.StyleCopPlusRules",
        "SP2100:CodeLineMustNotBeLongerThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private const string CsvMappingPattern =
            @"^CsvMapping\('codeconversion(?<Extension>(\.csv))?';'\{(?<Parameter>[2,3])\}';\$Route\.Line{default};0:\$Route\.SpecialLine{default};1:\$Route\.Line{default}\)$";

        private static readonly Regex CsvMappingRegex = new Regex(CsvMappingPattern, RegexOptions.IgnoreCase);

        /// <summary>
        /// Converts legacy CsvMapping to CodeConversion and removes the '.csv' extension from all Csv mapping
        /// evaluations in the project.
        /// </summary>
        /// <param name="project">The project containing the evaluations to update.</param>
        protected void UpdateCsvMapping(MediaProjectDataViewModel project)
        {
            UpdatePredefinedFormulae(project);

            foreach (var layout in project.InfomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements)
                    {
                        var audioOutputElement = element as AudioOutputElementDataViewModel;
                        if (audioOutputElement != null)
                        {
                            this.UpdateCsvMappingFormulaInObject(element);
                            foreach (var playbackElement in audioOutputElement.Elements)
                            {
                                this.UpdateCsvMappingFormulaInObject(playbackElement);
                            }
                        }
                        else
                        {
                            this.UpdateCsvMappingFormulaInObject(element);
                        }
                    }
                }
            }

            foreach (var standardCycle in project.InfomediaConfig.Cycles.StandardCycles)
            {
                foreach (var section in standardCycle.Sections)
                {
                    this.UpdateCsvMappingFormulaInObject(section);
                }

                this.UpdateCsvMappingFormulaInObject(standardCycle);
            }

            foreach (var eventCycle in project.InfomediaConfig.Cycles.EventCycles)
            {
                foreach (var section in eventCycle.Sections)
                {
                    this.UpdateCsvMappingFormulaInObject(section);
                }

                this.UpdateCsvMappingFormulaInObject(eventCycle);
            }

            foreach (var screen in project.InfomediaConfig.PhysicalScreens)
            {
                this.UpdateCsvMappingFormulaInObject(screen);
            }
        }

        private static void UpdatePredefinedFormulae(MediaProjectDataViewModel project)
        {
            var shell = ServiceLocator.Current.GetInstance<IMediaShell>();
            foreach (var evaluationConfigDataViewModel in project.InfomediaConfig.Evaluations)
            {
                var csvMapping = evaluationConfigDataViewModel.Evaluation as CsvMappingEvalDataViewModel;
                if (csvMapping == null)
                {
                    continue;
                }

                bool isImage;
                if (IsLegacyCsvMapping(csvMapping, out isImage))
                {
                    evaluationConfigDataViewModel.Evaluation = new CodeConversionEvalDataViewModel(shell)
                    {
                        FileName = NewDataValue("codeconversion.csv"),
                        UseImage = NewDataValue(isImage)
                    };
                    continue;
                }

                csvMapping.FileName.Value = csvMapping.FileName.Value.Replace(".csv", string.Empty);
            }
        }

        private static DataValue<T> NewDataValue<T>(T value)
        {
            return new DataValue<T>(value);
        }

        private static bool IsLegacyCsvMapping(EvalDataViewModelBase evaluation, out bool isImage)
        {
            if (evaluation is CsvMappingEvalDataViewModel)
            {
                var humanReadable = evaluation.HumanReadable().Replace(" ", string.Empty);
                var match = CsvMappingRegex.Match(humanReadable);
                if (match.Success)
                {
                    isImage = match.Groups["Parameter"].Value == "2";
                    return true;
                }
            }

            isImage = false;
            return false;
        }

        private void UpdateCsvMappingFormulaInObject(object obj)
        {
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var dynamicProperty = propertyInfo.GetValue(obj, new object[0]) as IDynamicDataValue;
                if (dynamicProperty == null || dynamicProperty.Formula == null)
                {
                    continue;
                }

                var csvMappingFormula = dynamicProperty.Formula as CsvMappingEvalDataViewModel;
                if (csvMappingFormula != null)
                {
                    csvMappingFormula.FileName.Value = csvMappingFormula.FileName.Value.Replace(
                        ".csv",
                        string.Empty);
                    continue;
                }

                this.UpdateCsvMappingFormulaInObject(dynamicProperty.Formula);
            }
        }
    }
}