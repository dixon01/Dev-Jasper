// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaManagerPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaManagerPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The FormulaManagerPrompt.
    /// </summary>
    public class FormulaManagerPrompt : PromptNotification
    {
        private readonly IMediaShell shell;

        private readonly ICommandRegistry commandRegistry;

        private ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas;

        private FormulaEditorPrompt currentFormulaPrompt;

        private EvaluationConfigDataViewModel currentEvaluation;

        private EvaluationConfigDataViewModel currentUnchangedEvaluation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaManagerPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="commandRegistry">the command registry</param>
        public FormulaManagerPrompt(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.shell = shell;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the predefined formulas
        /// </summary>
        public ExtendedObservableCollection<EvaluationConfigDataViewModel> PredefinedFormulas
        {
            get
            {
                if (this.predefinedFormulas == null)
                {
                    var currentProject = this.shell.MediaApplicationState.CurrentProject;
                    if (currentProject != null)
                    {
                        this.predefinedFormulas = currentProject.InfomediaConfig.Evaluations;
                    }

                    this.shell.MediaApplicationState.PropertyChanged -= this.MediaApplicationStateOnPropertyChanged;
                    this.shell.MediaApplicationState.PropertyChanged += this.MediaApplicationStateOnPropertyChanged;
                }

                return this.predefinedFormulas;
            }

            set
            {
                this.SetProperty(ref this.predefinedFormulas, value, () => this.PredefinedFormulas);
            }
        }

        /// <summary>
        /// Gets or sets the current formula prompt
        /// </summary>
        public FormulaEditorPrompt CurrentFormulaPrompt
        {
            get
            {
                return this.currentFormulaPrompt;
            }

            set
            {
                this.SetProperty(ref this.currentFormulaPrompt, value, () => this.CurrentFormulaPrompt);
            }
        }

        /// <summary>
        /// Gets or sets the current formula prompt
        /// </summary>
        public EvaluationConfigDataViewModel CurrentEvaluation
        {
            get
            {
                return this.currentEvaluation;
            }

            set
            {
                this.SetProperty(ref this.currentEvaluation, value, () => this.CurrentEvaluation);

                if (this.currentEvaluation != null)
                {
                    this.currentUnchangedEvaluation = (EvaluationConfigDataViewModel)this.currentEvaluation.Clone();
                    this.CurrentFormulaPrompt = new FormulaEditorPrompt(
                        this.shell,
                        this.currentEvaluation,
                        this.commandRegistry)
                                               {
                                                   IsOpen = true,
                                               };
                }
                else
                {
                    this.CurrentFormulaPrompt = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current unchanged evaluation.
        /// </summary>
        public EvaluationConfigDataViewModel CurrentUnchangedEvaluation
        {
            get
            {
                return this.currentUnchangedEvaluation;
            }

            set
            {
                this.SetProperty(ref this.currentUnchangedEvaluation, value, () => this.CurrentUnchangedEvaluation);
            }
        }

        /// <summary>
        /// Gets the create command
        /// </summary>
        public ICommand CreatePredefinedFormula
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CreatePredefinedFormula);
            }
        }

        /// <summary>
        /// Gets the update command
        /// </summary>
        public ICommand UpdatePredefinedFormula
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.UpdatePredefinedFormula);
            }
        }

        /// <summary>
        /// Gets the Delete command
        /// </summary>
        public ICommand DeletePredefinedFormula
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeletePredefinedFormula);
            }
        }

        /// <summary>
        /// Gets the Clone command
        /// </summary>
        public ICommand ClonePredefinedFormula
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.ClonePredefinedFormula);
            }
        }

        /// <summary>
        /// Gets the Rename command
        /// </summary>
        public ICommand RenamePredefinedFormula
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.RenamePredefinedFormula);
            }
        }

        /// <summary>
        /// Gets the ShowDictionarySelector command
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
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid
        {
            get
            {
                var result = this.PredefinedFormulas.All(f => f.IsValid());
                return result;
            }
        }

        private void MediaApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentProject")
            {
                this.predefinedFormulas = null;
                this.RaisePropertyChanged(() => this.PredefinedFormulas);
            }
        }
    }
}