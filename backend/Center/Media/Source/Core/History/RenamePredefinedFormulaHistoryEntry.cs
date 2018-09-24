// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenamePredefinedFormulaHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    /// <summary>
    /// History entry that contains all information to undo / redo the renaming of a PredefinedFormula.
    /// </summary>
    public class RenamePredefinedFormulaHistoryEntry : HistoryEntryBase
    {
        private readonly EvaluationConfigDataViewModel element;
        private readonly string newName;

        private readonly Action<EvaluationConfigDataViewModel> postActionCallback;

        private string oldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenamePredefinedFormulaHistoryEntry"/> class.
        /// </summary>
        /// <param name="element"> The element to be renamed </param>
        /// <param name="newName"> The new name. </param>
        /// <param name="displayText"> The text to be displayed for this Entry on the UI. </param>
        /// <param name="postActionCallback">the callback to be called after do or undo</param>
        public RenamePredefinedFormulaHistoryEntry(
            EvaluationConfigDataViewModel element, 
            string newName, 
            string displayText, 
            Action<EvaluationConfigDataViewModel> postActionCallback)
            : base(displayText)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (newName == null)
            {
                throw new ArgumentNullException("newName");
            }

            this.element = element;
            this.newName = newName;
            this.postActionCallback = postActionCallback;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.element.MakeDirty();

            this.oldName = this.element.Name.Value;
            this.element.Name.Value = this.newName;
            this.element.DisplayText = this.newName;

            if (this.postActionCallback != null)
            {
                this.postActionCallback(this.element);
            }
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.element.MakeDirty();

            this.element.Name.Value = this.oldName;
            this.element.DisplayText = this.oldName;

            if (this.postActionCallback != null)
            {
                this.postActionCallback(this.element);
            }
        }
    }
}
