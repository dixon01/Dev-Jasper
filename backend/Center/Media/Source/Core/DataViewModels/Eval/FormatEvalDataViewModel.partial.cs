// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Extends the generated model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The format evaluation data view model.
    /// </summary>
    public partial class FormatEvalDataViewModel
    {
        private ExtendedObservableCollection<EvaluationConfigDataViewModel> evalArguments;

        private bool isArgumentsChanging;

        private bool isEvalArgumentsChanging;

        /// <summary>
        /// Gets or sets the evaluation arguments used as wrapper for the formula editor.
        /// </summary>
        [UserVisibleProperty("Eval")]
        public ExtendedObservableCollection<EvaluationConfigDataViewModel> EvaluationArguments
        {
            get
            {
                return this.evalArguments;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.evalArguments);
                if (this.evalArguments != null)
                {
                    this.evalArguments.CollectionChanged -= this.EvaluationArgumentsChanged;
                }

                this.SetProperty(ref this.evalArguments, value, () => this.EvaluationArguments);

                if (value != null)
                {
                    this.evalArguments.CollectionChanged += this.EvaluationArgumentsChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var formatValue = string.Empty;
            if (this.Format != null)
            {
                formatValue = this.Format.Value;
            }

            var operands = string.Empty;
            if (this.arguments != null && this.Arguments.Count > 0)
            {
                operands = "; " + string.Join(
                    "; ", this.Arguments.Select(o => o == null ? string.Empty : o.HumanReadable()));
            }

            return "Format ( '" + formatValue + "'" + operands + " )";
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

            foreach (var argument in this.Arguments)
            {
                if (argument != null)
                {
                    result.AddRange(argument.GetContainedPredefinedFormulas());
                }
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

            for (var i = 0; i < this.Arguments.Count; i++)
            {
                if (this.Arguments[i] != null)
                {
                    if (this.Arguments[i] is EvaluationEvalDataViewModel && this.Arguments[i].ClonedFrom != 0)
                    {
                        var predefinedFormula = ((EvaluationEvalDataViewModel)this.Arguments[i]).Reference;
                        if (predefinedFormula != null)
                        {
                            predefinedFormula.ReferencesCount--;
                            result.Add(predefinedFormula);
                        }
                    }
                    else
                    {
                        this.Arguments[i].ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The clear empty arguments.
        /// </summary>
        public void ClearEmptyArguments()
        {
            for (var i = this.EvaluationArguments.Count - 1; 0 <= i; i--)
            {
                if (this.EvaluationArguments[i].Evaluation == null
                    || this.EvaluationArguments[i].Evaluation.HumanReadable().Equals(string.Empty))
                {
                    this.isEvalArgumentsChanging = true;
                    this.EvaluationArguments.RemoveAt(i);
                    this.isEvalArgumentsChanging = false;
                }
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        protected override IEnumerable<string> Validate(string propertyName)
        {
            var errorList = base.Validate(propertyName);

            var missingConditions = this.arguments.Count == 0;
            var hasNullConditions = this.arguments.Any(c => c == null);

            if (missingConditions || hasNullConditions)
            {
                var extendedErrorList = errorList.ToList();

                if (missingConditions)
                {
                    extendedErrorList.Add(
                        string.Format(
                            MediaStrings.FormulaParser_Error_CollectionEmpty,
                            MediaStrings.FormulaEditor_Eval_Format));
                }

                if (hasNullConditions)
                {
                    extendedErrorList.Add(
                        string.Format(
                            MediaStrings.FormulaParser_Error_CollectionItemNull,
                            MediaStrings.FormulaEditor_Eval_Format));
                }

                errorList = extendedErrorList;
            }

            return errorList;
        }

        partial void Initialize(FormatEvalDataViewModel dataViewModel)
        {
            this.evalArguments = new ExtendedObservableCollection<EvaluationConfigDataViewModel>();
            this.evalArguments.CollectionChanged += this.EvaluationArgumentsChanged;
            this.Arguments.CollectionChanged += this.ConditionsChanged;
            if (dataViewModel != null)
            {
                foreach (var evalDataViewModelBase in this.Arguments)
                {
                    this.evalArguments.Add(
                        new EvaluationConfigDataViewModel(this.mediaShell) { Evaluation = evalDataViewModelBase });
                }
            }
        }

        partial void Initialize(Models.Eval.FormatEvalDataModel dataModel)
        {
            this.EvaluationArguments = new ExtendedObservableCollection<EvaluationConfigDataViewModel>();
            this.Arguments.CollectionChanged += this.ConditionsChanged;
            if (dataModel != null)
            {
                foreach (var evalDataViewModelBase in this.Arguments)
                {
                    this.evalArguments.Add(
                        new EvaluationConfigDataViewModel(this.mediaShell) { Evaluation = evalDataViewModelBase });
                }
            }
        }

        private void ConditionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.isEvalArgumentsChanging)
            {
                return;
            }

            this.isArgumentsChanging = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems.OfType<EvalDataViewModelBase>())
                {
                    foreach (var eval in this.EvaluationArguments)
                    {
                        if (eval.Evaluation == newItem)
                        {
                            this.isArgumentsChanging = false;
                            return;
                        }
                    }

                    var newEvaluation = new EvaluationConfigDataViewModel(this.mediaShell) { Evaluation = newItem };
                    newEvaluation.PropertyChanged += this.EvaluationPropertyChanged;
                    this.EvaluationArguments.Add(newEvaluation);
                }

                this.isArgumentsChanging = false;
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var item =
                        this.EvaluationArguments.FirstOrDefault(
                            i =>
                            i.Evaluation != null
                            && i.Evaluation.HumanReadable() == ((EvalDataViewModelBase)oldItem).HumanReadable());
                    if (item != null)
                    {
                        this.EvaluationArguments.Remove(item);
                    }
                }
            }

            this.isArgumentsChanging = false;
        }

        private void EvaluationArgumentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.isArgumentsChanging)
            {
                return;
            }

            this.isEvalArgumentsChanging = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems.OfType<EvaluationConfigDataViewModel>())
                {
                    if (newItem.Evaluation != null && !this.Arguments.Contains(newItem.Evaluation))
                    {
                        this.Arguments.Add(newItem.Evaluation);
                    }

                    newItem.PropertyChanged += this.EvaluationPropertyChanged;
                }

                this.isEvalArgumentsChanging = false;
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    if (((EvaluationConfigDataViewModel)oldItem).Evaluation == null)
                    {
                        if (this.Arguments.Count <= e.OldStartingIndex || this.Arguments[e.OldStartingIndex] != null)
                        {
                            throw new Exception("Can not remove argument, index not valid.");
                        }

                        this.Arguments.RemoveAt(e.OldStartingIndex);
                        return;
                    }

                    var item =
                        this.Arguments.FirstOrDefault(
                            i =>
                            i.HumanReadable() == ((EvaluationConfigDataViewModel)oldItem).Evaluation.HumanReadable());
                    if (item != null)
                    {
                        this.Arguments.Remove(item);
                    }
                }
            }

            this.isEvalArgumentsChanging = false;
        }

        private void EvaluationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var eval = (EvaluationConfigDataViewModel)sender;
            if (this.Arguments.Count != this.evalArguments.Count)
            {
                this.Arguments.Add(eval.Evaluation);
                return;
            }

            var index = this.evalArguments.IndexOf(eval);
            this.Arguments[index] = eval.Evaluation;
        }

        partial void ExportNotGeneratedValues(FormatEval model, object exportParameters)
        {
            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            for (int i = 0; i < model.Arguments.Count; i++)
            {
                var eval = model.Arguments[i] as CodeConversionEval;
                if (eval != null)
                {
                    model.Arguments[i] = this.ConvertCodeConversionToCsvMapping(eval);
                }
            }
        }
    }
}
