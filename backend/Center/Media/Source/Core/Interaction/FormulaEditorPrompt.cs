// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaEditorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormulaEditorPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// the Formula Editor Prompt
    /// </summary>
    public class FormulaEditorPrompt : PromptNotification, IDataErrorInfo
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICommandRegistry commandRegistry;

        private IDynamicDataValue dataValue;

        private EvaluationType selectedEvaluationType;

        private EvaluationConfigDataViewModel selectedPredefinedFormula;

        private EvaluationConfigDataViewModel evaluation;

        private string expertEvaluationPart;

        private bool isFormulaChangedFromExpertMode;

        private bool isFormulaChangedFromSimpleMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaEditorPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="dataValue">
        /// the data value
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public FormulaEditorPrompt(IMediaShell shell, IDynamicDataValue dataValue, ICommandRegistry commandRegistry)
        {
            this.DataValue = dataValue;
            this.Shell = shell;
            this.commandRegistry = commandRegistry;
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaEditorPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="evaluation">
        /// the evaluation
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public FormulaEditorPrompt(
            IMediaShell shell,
            EvaluationConfigDataViewModel evaluation,
            ICommandRegistry commandRegistry)
            : this(shell, new DynamicDataValue<string> { Formula = evaluation.Evaluation }, commandRegistry)
        {
            this.evaluation = evaluation;
        }

        /// <summary>
        /// Gets or sets a value indicating whether has pending changes.
        /// </summary>
        public bool HasPendingChanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a formula has changed in expert mode.
        /// </summary>
        public bool HasChangedInExpertMode { get; set; }

        /// <summary>
        /// Gets or sets the selected evaluation type
        /// </summary>
        public EvaluationType SelectedEvaluationType
        {
            get
            {
                return this.selectedEvaluationType;
            }

            set
            {
                this.SetProperty(ref this.selectedEvaluationType, value, () => this.SelectedEvaluationType);
            }
        }

        /// <summary>
        /// Gets or sets the selected evaluation type
        /// </summary>
        public EvaluationConfigDataViewModel SelectedPredefinedFormula
        {
            get
            {
                return this.selectedPredefinedFormula;
            }

            set
            {
                this.SetProperty(ref this.selectedPredefinedFormula, value, () => this.SelectedPredefinedFormula);
            }
        }

        /// <summary>
        /// Gets or sets the expert evaluation part.
        /// </summary>
        public string ExpertEvaluationPart
        {
            get
            {
                return this.expertEvaluationPart;
            }

            set
            {
                if (this.isFormulaChangedFromSimpleMode)
                {
                    this.RaisePropertyChanged(() => this.ExpertEvaluationPart);
                    return;
                }

                this.isFormulaChangedFromExpertMode = true;
                this.SetProperty(ref this.expertEvaluationPart, value, () => this.ExpertEvaluationPart);
                if (value != null)
                {
                    var appController = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                    var formulaController = appController.ShellController.FormulaController;
                    try
                    {
                        var formula = value;
                        if (!value.StartsWith("="))
                        {
                            formula = "= " + value;
                        }

                        var result = formulaController.ParseFormula(formula);
                        this.Error = string.Empty;
                        this.SetEvaluation(result);
                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Unable to parse expert formula string {0}.", value);
                        Logger.DebugException(message, e);
                        this.Error = e.Message;
                    }
                }

                this.RaisePropertyChanged(() => this.ExpertEvaluationPart);
                this.isFormulaChangedFromExpertMode = false;
            }
        }

        /// <summary>
        /// Gets or sets the current DataValue
        /// </summary>
        public IDynamicDataValue DataValue
        {
            get
            {
                return this.dataValue;
            }

            set
            {
                this.SetProperty(ref this.dataValue, value, () => this.DataValue);
            }
        }

        /// <summary>
        /// Gets the show dictionary selector command.
        /// </summary>
        public ICommand ShowDictionarySelectorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowDictionarySelector);
            }
        }

        /// <summary>
        /// Gets the dictionary selector interaction request.
        /// </summary>
        public IInteractionRequest DictionarySelectorInteractionRequest
        {
            get
            {
                return InteractionManager<DictionarySelectorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the media shell
        /// </summary>
        public IMediaShell Shell { get; private set; }

        /// <summary>
        /// Gets the application controller.
        /// </summary>
        public IMediaApplicationController ApplicationController
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IMediaApplicationController>();
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string columnName]
        {
            get
            {
                return this.Error;
            }
        }

        /// <summary>
        /// sets the evaluation of the prompt
        /// </summary>
        /// <param name="eval">the evaluation</param>
        /// <param name="isFormulaEmpty"> Shows a message box that the formula is empty</param>
        public void SetEvaluation(EvalDataViewModelBase eval, bool isFormulaEmpty = false)
        {
            if (isFormulaEmpty)
            {
                MessageBox.Show(MediaStrings.FormulaEditor_EmptyPredefinedFormula);
            }

            this.DataValue.Formula = eval;

            if (this.isFormulaChangedFromExpertMode)
            {
                if (this.dataValue.Formula == null)
                {
                    return;
                }

                var type = this.dataValue.Formula.GetType().Name.Replace("EvalDataViewModel", string.Empty);
                EvaluationType evalType;
                Enum.TryParse(type, out evalType);

                var evaluationEvalDataViewModel = this.dataValue.Formula as EvaluationEvalDataViewModel;
                if (evaluationEvalDataViewModel != null)
                {
                    this.SelectedEvaluationType = EvaluationType.Evaluation;
                }
                else
                {
                    this.SelectedEvaluationType = evalType;
                }

                if (this.evaluation != null)
                {
                    this.evaluation.Evaluation = this.dataValue != null ? eval : null;
                }

                this.isFormulaChangedFromSimpleMode = false;
                return;
            }

            this.isFormulaChangedFromSimpleMode = true;
            if (this.evaluation != null)
            {
                this.evaluation.Evaluation = this.dataValue != null ? eval : null;
            }

            var evaluationConfig = this.DataValue.Formula as EvaluationEvalDataViewModel;

            if (!this.isFormulaChangedFromExpertMode)
            {
                if (evaluationConfig != null)
                {
                    this.ExpertEvaluationPart = evaluationConfig.Reference.Evaluation.HumanReadable();
                }
                else
                {
                    if (this.DataValue.Formula != null)
                    {
                        this.ExpertEvaluationPart = ((EvalDataViewModelBase)this.DataValue.Formula).HumanReadable();
                    }
                }
            }

            if (eval != null)
            {
                if (eval.IsValid())
                {
                    this.Error = string.Empty;
                }
            }

            this.isFormulaChangedFromSimpleMode = false;
        }

        private void Initialize()
        {
            if (this.dataValue.Formula == null)
            {
                return;
            }

            var type = this.dataValue.Formula.GetType().Name.Replace("EvalDataViewModel", string.Empty);
            EvaluationType evalType;
            Enum.TryParse(type, out evalType);
            this.selectedEvaluationType = evalType;

            var evaluationConfigDataViewModel = this.dataValue.Formula as EvaluationEvalDataViewModel;
            if (evaluationConfigDataViewModel != null)
            {
                this.selectedPredefinedFormula = evaluationConfigDataViewModel.Reference;
                this.selectedEvaluationType = EvaluationType.Evaluation;
                this.expertEvaluationPart = evaluationConfigDataViewModel.HumanReadable();
            }
            else
            {
                this.expertEvaluationPart = ((EvalDataViewModelBase)this.dataValue.Formula).HumanReadable();
            }

            this.HasPendingChanges = false;
        }
    }
}