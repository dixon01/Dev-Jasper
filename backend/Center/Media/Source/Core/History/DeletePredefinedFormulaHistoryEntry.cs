// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletePredefinedFormulaHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a replacement element.
    /// </summary>
    public class DeletePredefinedFormulaHistoryEntry : HistoryEntryBase
    {
        private readonly EvaluationConfigDataViewModel dataViewModel;

        private readonly ExtendedObservableCollection<EvaluationConfigDataViewModel> container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePredefinedFormulaHistoryEntry"/> class.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data View Model.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="displayText">
        /// The display text shown on the UI.
        /// </param>
        public DeletePredefinedFormulaHistoryEntry(
            EvaluationConfigDataViewModel dataViewModel,
            ExtendedObservableCollection<EvaluationConfigDataViewModel> container,
            string displayText)
            : base(displayText)
        {
            if (dataViewModel == null)
            {
                throw new ArgumentNullException("dataViewModel");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.dataViewModel = dataViewModel;
            this.container = container;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.dataViewModel.MakeDirty();
            this.container.Add(this.dataViewModel);
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.container.Remove(this.dataViewModel);
        }
    }
}