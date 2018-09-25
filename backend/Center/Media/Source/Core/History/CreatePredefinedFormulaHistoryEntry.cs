// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatePredefinedFormulaHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CreatePredefinedFormulaHistoryEntry.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    using NLog;

    /// <summary>
    /// The CreatePredefinedFormulaHistoryEntry.
    /// </summary>
    public class CreatePredefinedFormulaHistoryEntry : HistoryEntryBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly EvaluationConfigDataViewModel dataViewModel;

        private readonly ExtendedObservableCollection<EvaluationConfigDataViewModel> container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePredefinedFormulaHistoryEntry"/> class.
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
        public CreatePredefinedFormulaHistoryEntry(
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
        /// The Do function
        /// </summary>
        public override void Do()
        {
            this.dataViewModel.MakeDirty();
            this.container.Add(this.dataViewModel);
        }

        /// <summary>
        /// the undo function
        /// </summary>
        public override void Undo()
        {
            this.container.Remove(this.dataViewModel);
        }
    }
}