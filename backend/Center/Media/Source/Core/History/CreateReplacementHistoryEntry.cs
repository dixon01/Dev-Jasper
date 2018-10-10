// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateReplacementHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a replacement element.
    /// </summary>
    public class CreateReplacementHistoryEntry : HistoryEntryBase
    {
        private readonly TextualReplacementDataViewModel dataViewModel;

        private readonly ExtendedObservableCollection<TextualReplacementDataViewModel> container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateReplacementHistoryEntry"/> class.
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
        public CreateReplacementHistoryEntry(
            TextualReplacementDataViewModel dataViewModel,
            ExtendedObservableCollection<TextualReplacementDataViewModel> container,
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

            this.SetMediaReference();
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.container.Remove(this.dataViewModel);
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.dataViewModel.MakeDirty();
            this.container.Add(this.dataViewModel);

            this.UnsetMediaReference();
        }

        private void SetMediaReference()
        {
            if (this.dataViewModel.IsImageReplacement && this.dataViewModel.Image != null)
            {
                var selectionParameters = new SelectResourceParameters
                {
                    CurrentSelectedResourceHash = this.dataViewModel.Image.Hash
                };
                this.dataViewModel.CommandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
            }
        }

        private void UnsetMediaReference()
        {
            if (this.dataViewModel.IsImageReplacement && this.dataViewModel.Image != null)
            {
                var selectionParameters = new SelectResourceParameters
                {
                    PreviousSelectedResourceHash = this.dataViewModel.Image.Hash
                };
                this.dataViewModel.CommandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
            }
        }
    }
}