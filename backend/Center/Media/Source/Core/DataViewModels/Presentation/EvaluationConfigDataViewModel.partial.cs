// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EvaluationConfigDataViewModel.partial.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using NLog;

    /// <summary>
    /// The EvaluationConfigDataViewModel.
    /// </summary>
    public partial class EvaluationConfigDataViewModel : IReusableEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool isInEditMode;

        private int referencesCount;

        private string isUsedToolTip;

        /// <summary>
        /// Gets or sets the reference count
        /// </summary>
        public int ReferencesCount
        {
            get
            {
                return this.referencesCount;
            }

            set
            {
                var valueToSet = value >= 0 ? value : 0;
                this.SetProperty(ref this.referencesCount, valueToSet, () => this.ReferencesCount);
                this.IsUsedToolTip = string.Format(MediaStrings.Evaluation_UsedLayoutTooltip, this.referencesCount);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object is in edit mode
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                this.SetProperty(ref this.isInEditMode, value, () => this.IsInEditMode);
            }
        }

        /// <summary>
        /// Gets or sets the tooltip that is shown if the object is used
        /// </summary>
        public string IsUsedToolTip
        {
            get
            {
                return this.isUsedToolTip;
            }

            set
            {
                this.SetProperty(ref this.isUsedToolTip, value, () => this.IsUsedToolTip);
            }
        }

        /// <summary>
        /// Get the name of the object
        /// </summary>
        /// <returns>the name</returns>
        public string GetName()
        {
            return this.Name.Value;
        }

        /// <summary>
        /// Sets the name
        /// </summary>
        /// <param name="newName">the new name</param>
        public void SetName(string newName)
        {
            this.Name = new DataValue<string>(newName);
        }

        /// <summary>
        /// creates a human readable string representation
        /// </summary>
        /// <returns>the readable string</returns>
        public override string HumanReadable()
        {
            return (this.Name == null || string.IsNullOrEmpty(this.Name.Value)) ?
                string.Empty :
                this.Name.Value + "()";
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>the name</returns>
        public override string ToString()
        {
            return this.Name.Value;
        }

        /// <summary>
        /// Searches for all contained predefined formulas.
        /// </summary>
        /// <returns>
        /// The contained predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas()
        {
            return new List<EvaluationConfigDataViewModel> { this };
        }

        /// <summary>
        /// The set contained predefined formulas.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas)
        {
            return new List<EvaluationConfigDataViewModel>();
        }

        /// <summary>
        /// Validates the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The list of error messages for the given properties. Empty enumeration if no error was found.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<string> Validate(string propertyName)
        {
            if (propertyName == "Name")
            {
                var predefinedFormuals =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations.Where(
                        f => f.Name.Value == this.Name.Value);
                if (predefinedFormuals.Count() > 1)
                {
                    return new[] { MediaStrings.Evaluation_UniqueName };
                }

                var valid =
                    ((EvaluationType[])Enum.GetValues(typeof(EvaluationType))).All(
                        methodName => !this.Name.Value.Equals(methodName.ToString()));
                if (!valid)
                {
                    return new[] { MediaStrings.Evaluation_SystemNamesNotAllowed };
                }

                var invalidSpecialChars = Settings.Default.InvalidFormulaNameChars;
                valid = !invalidSpecialChars.Any(this.Name.Value.Contains);
                if (!valid)
                {
                    return new[] { MediaStrings.Evaluation_NoSpecialCharactersInName };
                }
            }

            return Enumerable.Empty<string>();
        }

        partial void Initialize(EvaluationConfigDataModel dataModel)
        {
            if (dataModel != null)
            {
                this.ReferencesCount = dataModel.ReferenceCount;
            }
        }

        partial void Initialize(EvaluationConfigDataViewModel dataViewModel)
        {
            this.ReferencesCount = dataViewModel.ReferencesCount;
        }

        partial void ConvertNotGeneratedToDataModel(ref EvaluationConfigDataModel dataModel)
        {
            if (dataModel != null)
            {
                dataModel.ReferenceCount = this.ReferencesCount;
            }
        }
    }
}