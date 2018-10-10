// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateLayoutHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a layout element.
    /// </summary>
    public class CreateLayoutHistoryEntry : HistoryEntryBase
    {
        private readonly LayoutConfigDataViewModel layout;
        private readonly IMediaApplicationState state;
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLayoutHistoryEntry"/> class.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if layout, container or project is null.
        /// </exception>
        public CreateLayoutHistoryEntry(
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
            if (this.state.CurrentLayout == this.layout)
            {
                this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                    .Execute(
                        this.state.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault(
                            l => !object.ReferenceEquals(this.layout, l)));
            }

            this.state.CurrentProject.InfomediaConfig.Layouts.Remove(this.layout);
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.state.CurrentProject.InfomediaConfig.Layouts.Add(this.layout);
        }
    }
}