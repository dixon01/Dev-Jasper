// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteLayoutHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//  Deletes a layout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a layout element.
    /// </summary>
    public class DeleteLayoutHistoryEntry : HistoryEntryBase
    {
        private readonly LayoutConfigDataViewModel layout;
        private readonly IMediaApplicationState state;
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteLayoutHistoryEntry"/> class.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Exception if argument is null.
        /// </exception>
        public DeleteLayoutHistoryEntry(
            LayoutConfigDataViewModel layout,
            IMediaApplicationState state,
            ICommandRegistry commandRegistry,
            string displayText)
            : base(displayText)
        {
            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            this.layout = layout;
            this.state = state;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.state.CurrentProject.InfomediaConfig.Layouts.Add(this.layout);

            HistoryHelper.SetPredefinedFormulaReferences(this.layout, this.state);
            HistoryHelper.SetMediaReferences(this.layout, this.commandRegistry);
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            if (this.state.CurrentLayout == this.layout)
            {
                this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                    .Execute(
                        this.state.CurrentLayout =
                        this.state.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault(
                            l => !object.ReferenceEquals(this.layout, l)));
            }

            this.state.CurrentProject.InfomediaConfig.Layouts.Remove(this.layout);

            HistoryHelper.UnsetPredefinedFormulaReferences(this.layout, this.state);
            HistoryHelper.UnsetMediaReferences(this.layout, this.commandRegistry);
        }
    }
}