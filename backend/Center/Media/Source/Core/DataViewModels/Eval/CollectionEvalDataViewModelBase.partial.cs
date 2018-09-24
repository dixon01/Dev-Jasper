// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionEvalDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The collection eval data view model base.
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
    /// The collection evaluation data view model base.
    /// </summary>
    public partial class CollectionEvalDataViewModelBase
    {
        private ExtendedObservableCollection<EvaluationConfigDataViewModel> evalConditions;

        private bool isConditionsChanging;

        private bool isEvalConditionsChanging;

        /// <summary>
        /// Gets or sets the evaluation conditions used as wrapper for the formula editor.
        /// </summary>
        [UserVisibleProperty("Eval")]
        public ExtendedObservableCollection<EvaluationConfigDataViewModel> EvaluationConditions
        {
            get
            {
                return this.evalConditions;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.evalConditions);
                if (this.evalConditions != null)
                {
                    this.evalConditions.CollectionChanged -= this.EvaluationConditionsChanged;
                }

                this.SetProperty(ref this.evalConditions, value, () => this.EvaluationConditions);

                if (value != null)
                {
                    this.evalConditions.CollectionChanged += this.EvaluationConditionsChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
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

            foreach (var condition in this.Conditions)
            {
                if (condition != null)
                {
                    result.AddRange(condition.GetContainedPredefinedFormulas());
                }
            }

            return result;
        }

        /// <summary>
        /// The clear empty conditions.
        /// </summary>
        public void ClearEmptyConditions()
        {
            for (var i = this.EvaluationConditions.Count - 1; 0 <= i; i--)
            {
                if (this.EvaluationConditions[i].Evaluation == null
                    || this.EvaluationConditions[i].Evaluation.HumanReadable().Equals(string.Empty))
                {
                    this.isEvalConditionsChanging = true;
                    this.EvaluationConditions.RemoveAt(i);
                    this.isEvalConditionsChanging = false;
                }
            }
        }

        /// <summary>
        /// The set contained predefined formulas.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <returns>
        /// List of the original predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            for (var i = 0; i < this.Conditions.Count; i++)
            {
                if (this.Conditions[i] != null)
                {
                    if (this.Conditions[i] is EvaluationEvalDataViewModel && this.Conditions[i].ClonedFrom != 0)
                    {
                        var predefinedFormula = ((EvaluationEvalDataViewModel)this.Conditions[i]).Reference;
                        if (predefinedFormula != null)
                        {
                            predefinedFormula.ReferencesCount--;
                            result.Add(predefinedFormula);
                        }
                    }
                    else
                    {
                        this.Conditions[i].ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                    }
                }
            }

            return result;
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
            return this.Validate(propertyName, string.Empty);
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="collectionName">
        /// The collection name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        protected IEnumerable<string> Validate(string propertyName, string collectionName)
        {
            var errorList = base.Validate(propertyName);

            var missingConditions = this.conditions.Count == 0;
            var hasNullConditions = this.conditions.Any(c => c == null);
            var hasConstant = this.conditions.Any(c => c is ConstantEvalDataViewModel);

            if (missingConditions || hasNullConditions || hasConstant)
            {
                var extendedErrorList = errorList.ToList();

                if (missingConditions)
                {
                    extendedErrorList.Add(
                        string.Format(MediaStrings.FormulaParser_Error_CollectionEmpty, collectionName));
                }

                if (hasNullConditions)
                {
                    extendedErrorList.Add(
                        string.Format(MediaStrings.FormulaParser_Error_CollectionItemNull, collectionName));
                }

                if (hasConstant)
                {
                    extendedErrorList.Add(
                        string.Format(MediaStrings.FormulaParser_Error_CollectionItemConstant, collectionName));
                }

                errorList = extendedErrorList;
            }

            return errorList;
        }

        partial void Initialize(CollectionEvalDataViewModelBase dataViewModel)
        {
            this.evalConditions = new ExtendedObservableCollection<EvaluationConfigDataViewModel>();
            this.evalConditions.CollectionChanged += this.EvaluationConditionsChanged;
            this.Conditions.CollectionChanged += this.ConditionsChanged;
            if (dataViewModel != null)
            {
                foreach (var evalDataViewModelBase in this.Conditions)
                {
                    this.evalConditions.Add(
                        new EvaluationConfigDataViewModel(this.mediaShell) { Evaluation = evalDataViewModelBase });
                }
            }
        }

        partial void Initialize(Models.Eval.CollectionEvalDataModelBase dataModel)
        {
            this.EvaluationConditions = new ExtendedObservableCollection<EvaluationConfigDataViewModel>();
            if (dataModel != null)
            {
                foreach (var evalDataViewModelBase in this.Conditions)
                {
                    this.evalConditions.Add(
                        new EvaluationConfigDataViewModel(this.mediaShell) { Evaluation = evalDataViewModelBase });
                }
            }

            this.Conditions.CollectionChanged += this.ConditionsChanged;
        }

        private void ConditionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.isEvalConditionsChanging)
            {
                return;
            }

            this.isConditionsChanging = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems.OfType<EvalDataViewModelBase>())
                {
                    foreach (var eval in this.EvaluationConditions)
                    {
                        if (eval.Evaluation == newItem)
                        {
                            this.isConditionsChanging = false;
                            return;
                        }
                    }

                    var newEvaluation = new EvaluationConfigDataViewModel(this.mediaShell) { Evaluation = newItem };
                    newEvaluation.PropertyChanged += this.EvaluationPropertyChanged;
                    this.EvaluationConditions.Add(newEvaluation);
                }

                this.isConditionsChanging = false;
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var item =
                        this.EvaluationConditions.FirstOrDefault(
                            i =>
                            i.Evaluation != null
                            && i.Evaluation.HumanReadable() == ((EvalDataViewModelBase)oldItem).HumanReadable());
                    if (item != null)
                    {
                        this.EvaluationConditions.Remove(item);
                    }
                }
            }

            this.isConditionsChanging = false;
        }

        private void EvaluationConditionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.isConditionsChanging)
            {
                return;
            }

            this.isEvalConditionsChanging = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems.OfType<EvaluationConfigDataViewModel>())
                {
                    if (newItem.Evaluation != null)
                    {
                        var newEvaluation = newItem.Evaluation;
                        if (newEvaluation is EvaluationConfigDataViewModel)
                        {
                            var newPredefinedEval = new EvaluationEvalDataViewModel(this.mediaShell)
                                                        {
                                                            Reference = (EvaluationConfigDataViewModel)newEvaluation
                                                        };
                            var existing =
                                this.Conditions.FirstOrDefault(
                                    c =>
                                    c is EvaluationEvalDataViewModel
                                    && ((EvaluationEvalDataViewModel)c).Reference == newEvaluation);
                            if (existing == null)
                            {
                                this.Conditions.Add(newPredefinedEval);
                            }
                        }
                        else
                        {
                            if (newItem.Evaluation != null && !this.Conditions.Contains(newItem.Evaluation))
                            {
                                this.Conditions.Add(newItem.Evaluation);
                            }
                        }
                    }

                    newItem.PropertyChanged += this.EvaluationPropertyChanged;
                }

                this.isEvalConditionsChanging = false;
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    if (((EvaluationConfigDataViewModel)oldItem).Evaluation == null)
                    {
                        if (this.Conditions.Count <= e.OldStartingIndex || this.Conditions[e.OldStartingIndex] != null)
                        {
                            throw new Exception("Can not remove formula, index not valid.");
                        }

                        this.Conditions.RemoveAt(e.OldStartingIndex);
                        return;
                    }

                    var item =
                        this.Conditions.FirstOrDefault(
                            i =>
                                i != null
                                && i.HumanReadable()
                                == ((EvaluationConfigDataViewModel)oldItem).Evaluation.HumanReadable());
                    if (item != null)
                    {
                        this.Conditions.Remove(item);
                    }
                }
            }

            this.isEvalConditionsChanging = false;
        }

        private void EvaluationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var eval = (EvaluationConfigDataViewModel)sender;
            if (this.Conditions.Count != this.evalConditions.Count)
            {
                if (eval.Evaluation != null)
                {
                    var predefined = eval.Evaluation as EvaluationConfigDataViewModel;
                    if (predefined != null)
                    {
                        this.Conditions.Add(new EvaluationEvalDataViewModel(this.mediaShell)
                                                {
                                                    Reference = predefined
                                                });
                    }
                    else
                    {
                        this.Conditions.Add(eval.Evaluation);
                    }
                }
                else
                {
                    this.Conditions.Add(eval.Evaluation);
                }

                return;
            }

            var index = this.evalConditions.IndexOf(eval);
            if (eval.Evaluation != null)
            {
                var predefined = eval.Evaluation as EvaluationConfigDataViewModel;
                if (predefined != null)
                {
                    this.Conditions[index] =
                        new EvaluationEvalDataViewModel(this.mediaShell) { Reference = predefined };
                    return;
                }
            }

            this.Conditions[index] = eval.Evaluation;
        }

        partial void ExportNotGeneratedValues(CollectionEvalBase model, object exportParameters)
        {
            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            for (int i = 0; i < model.Conditions.Count; i++)
            {
                var conditionEval = model.Conditions[i] as CodeConversionEval;
                if (conditionEval != null)
                {
                    model.Conditions[i] = this.ConvertCodeConversionToCsvMapping(conditionEval);
                }
            }
        }
    }
}
