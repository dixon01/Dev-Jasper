// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClonePredefinedFormulaHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a layout element.
    /// </summary>
    public class ClonePredefinedFormulaHistoryEntry : HistoryEntryBase
    {
        private readonly EvaluationConfigDataViewModel evaluation;

        private readonly IMediaShell shell;

        private readonly ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulasContainer;

        private EvaluationConfigDataViewModel clonedEvaluation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClonePredefinedFormulaHistoryEntry"/> class.
        /// </summary>
        /// <param name="evaluation">
        ///     The evaluation to be cloned.
        /// </param>
        /// <param name="shell">the shell</param>
        /// <param name="predefinedFormulasContainer">the predefinedFormulasContainer</param>
        /// <param name="displayText">
        ///     The text to be displayed for this Entry on the UI.
        /// </param>
        public ClonePredefinedFormulaHistoryEntry(
            EvaluationConfigDataViewModel evaluation,
            IMediaShell shell,
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulasContainer,
            string displayText)
            : base(displayText)
        {
            if (evaluation == null)
            {
                throw new ArgumentNullException("evaluation");
            }

            if (shell == null)
            {
                throw new ArgumentNullException("shell");
            }

            this.evaluation = evaluation;
            this.shell = shell;
            this.predefinedFormulasContainer = predefinedFormulasContainer;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            var eval = (EvaluationConfigDataViewModel)this.evaluation.Clone();
            var newNameBase = string.Format(
                "{0}{1}",
                MediaStrings.FormulaManagerController_CloneNamePrefix,
                eval.Name.Value);
            var isUnique =
                !this.predefinedFormulasContainer.Any(
                    f => f.Name.Value.Equals(newNameBase, StringComparison.CurrentCultureIgnoreCase));
            var index = 1;
            var name = newNameBase;
            while (!isUnique)
            {
                index++;
                name = string.Format("{0}_{1}", newNameBase, index);
                isUnique =
                    !this.predefinedFormulasContainer.Any(
                        f => f.Name.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }

            eval.Name.Value = name;
            eval.DisplayText = eval.Name.Value;
            eval.IsInEditMode = true;

            this.predefinedFormulasContainer.Add(eval);
            this.clonedEvaluation = eval;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.predefinedFormulasContainer.Remove(this.clonedEvaluation);
        }
    }
}
