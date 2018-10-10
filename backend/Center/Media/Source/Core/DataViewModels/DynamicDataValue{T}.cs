// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicDataValue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicDataValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Defines a <see cref="DataValue&lt;T&gt;"/> specific for dynamic properties.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class DynamicDataValue<T> : DataValue<T>, IDynamicDataValue
    {
        private FormulaDataViewModelBase formula;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataValue{T}"/> class.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation formula.
        /// </param>
        public DynamicDataValue(EvalDataViewModelBase evaluation)
        {
            this.Formula = evaluation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataValue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public DynamicDataValue(T value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataValue&lt;T&gt;"/> class.
        /// </summary>
        public DynamicDataValue()
        {
        }

        /// <summary>
        /// Gets or sets the evaluation of the dynamic property.
        /// </summary>
        public FormulaDataViewModelBase Formula
        {
            get
            {
                return this.formula;
            }

            set
            {
                this.DecreaseReferenceCount(this.formula);
                this.SetProperty(ref this.formula, value, () => this.Formula);
                this.MakeDirty();
                this.IncreaseReferenceCount(this.formula);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value or the Formula is dirty.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || (this.Formula != null && this.Formula.IsDirty);
            }
        }

        /// <summary>
        /// Clears the dirty flag of the value and formula.
        /// </summary>
        public override void ClearDirty()
        {
            if (this.Formula != null)
            {
                this.Formula.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The cloned object.
        /// </returns>
        public override object Clone()
        {
            var clone = new DynamicDataValue<T> { Value = this.Value, };
            if (this.Formula != null)
            {
                var eval = (EvalDataViewModelBase)this.Formula;
                clone.Formula = (EvalDataViewModelBase)eval.Clone();
                clone.RestoreFormulaReferences();
            }

            return clone;
        }

        /// <summary>
        /// Compares the current object with another.
        /// </summary>
        /// <param name="that">the other data Value</param>
        /// <returns>
        /// true if they are equal
        /// </returns>
        public override bool EqualsValue(DataValue<T> that)
        {
            var result = base.EqualsValue(that);

            if (result && this.Formula != null)
            {
                result = this.Formula.EqualsViewModel(((DynamicDataValue<T>)that).Formula);
            }

            return result;
        }

        /// <summary>
        /// The restore formula references.
        /// </summary>
        public void RestoreFormulaReferences()
        {
            var predefinedFormula = this.formula as EvaluationEvalDataViewModel;

            if (predefinedFormula != null)
            {
                var reference = predefinedFormula.Reference;
                if (predefinedFormula.ClonedFrom != 0 && predefinedFormula.Reference != null)
                {
                    predefinedFormula.Reference.ReferencesCount--;
                }
            }
            else if (this.formula != null)
            {
                // ReSharper disable once UnusedVariable
                var replacedPredefinedformulas = this.RestoreContainedPredefinedFormulas(this.formula);
            }
        }

        /// <summary>
        /// The raise formula changed.
        /// </summary>
        public void RaiseFormulaChanged()
        {
            this.RaisePropertyChanged(() => this.Formula);
        }

        private void DecreaseReferenceCount(FormulaDataViewModelBase formulaDataViewModelBase)
        {
            var predefinedFormula = formulaDataViewModelBase as EvaluationEvalDataViewModel;
            if (predefinedFormula != null)
            {
                if (predefinedFormula.Reference != null)
                {
                    predefinedFormula.Reference.ReferencesCount--;
                }
            }
            else if (formulaDataViewModelBase != null)
            {
                var predefinedFormulas = this.GetPredefinedFormulas(formulaDataViewModelBase);
                foreach (var containedPredefinedFormula in predefinedFormulas)
                {
                    containedPredefinedFormula.ReferencesCount--;
                }
            }
        }

        private void IncreaseReferenceCount(FormulaDataViewModelBase formulaDataViewModelBase)
        {
            var predefinedFormula = formulaDataViewModelBase as EvaluationEvalDataViewModel;
            if (predefinedFormula != null)
            {
                if (predefinedFormula.Reference != null)
                {
                    predefinedFormula.Reference.ReferencesCount++;
                }
            }
            else if (formulaDataViewModelBase != null)
            {
                var predefinedFormulas = this.GetPredefinedFormulas(formulaDataViewModelBase);
                foreach (var containedPredefinedFormula in predefinedFormulas)
                {
                    containedPredefinedFormula.ReferencesCount++;
                }
            }
        }

        /// <summary>
        /// The get predefined formulas.
        /// </summary>
        /// <param name="formulaDataViewModelBase">
        /// The formula data view model base.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<EvaluationConfigDataViewModel> GetPredefinedFormulas(
            FormulaDataViewModelBase formulaDataViewModelBase)
        {
            IEnumerable<EvaluationConfigDataViewModel> result;

            var evalBase = formulaDataViewModelBase as EvalDataViewModelBase;
            if (evalBase != null)
            {
                result = evalBase.GetContainedPredefinedFormulas();
            }
            else
            {
                result = Enumerable.Empty<EvaluationConfigDataViewModel>();
            }

            return result;
        }

        private IEnumerable<EvaluationConfigDataViewModel> RestoreContainedPredefinedFormulas(
            FormulaDataViewModelBase formulaDataViewModelBase)
        {
            IEnumerable<EvaluationConfigDataViewModel> result;
            var predefinedFormulas =
                ServiceLocator.Current.GetInstance<IMediaApplicationState>().CurrentProject.InfomediaConfig.Evaluations;

            var evalBase = formulaDataViewModelBase as EvalDataViewModelBase;
            if (evalBase != null)
            {
                result = evalBase.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
            }
            else
            {
                result = new List<EvaluationConfigDataViewModel>();
            }

            return result;
        }

        private EvaluationConfigDataViewModel GetOriginalPredefinedFormula(int hash)
        {
            EvaluationConfigDataViewModel result = null;
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            foreach (var predefinedFormula in state.CurrentProject.InfomediaConfig.Evaluations)
            {
                if (predefinedFormula.GetHashCode() == hash)
                {
                    result = predefinedFormula;
                }
            }

            return result;
        }
    }
}
