// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloneLayoutHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a layout.
    /// </summary>
    public class CloneLayoutHistoryEntry : HistoryEntryBase
    {
        private readonly LayoutConfigDataViewModelBase layout;

        private readonly IMediaApplicationState state;

        private readonly ICommandRegistry commandRegistry;

        private LayoutConfigDataViewModel clonedLayout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloneLayoutHistoryEntry"/> class.
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
        /// All parameters are required.
        /// </exception>
        public CloneLayoutHistoryEntry(
            LayoutConfigDataViewModelBase layout,
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

            if (commandRegistry == null)
            {
                throw new ArgumentNullException("commandRegistry");
            }

            this.layout = layout;
            this.state = state;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            var currentProject = this.state.CurrentProject.InfomediaConfig;
            var clone = (LayoutConfigDataViewModel)this.layout.Clone();

            var index = 0;
            var hasUniqueName = false;
            var baseName = string.Format(
                "{0} {1}", MediaStrings.LayoutNavigationDialog_CloneNamePrefix, clone.Name.Value);
            var newName = string.Empty;
            while (!hasUniqueName)
            {
                index++;
                newName = baseName;

                if (index > 1)
                {
                    newName += " (" + index + ")";
                }

                hasUniqueName = !currentProject.Layouts.Any(l => l.Name.Value.Equals(newName));
            }

            // clones need to increase references too
            HistoryHelper.SetPredefinedFormulaReferences(clone, this.state);
            HistoryHelper.SetMediaReferences(clone, this.commandRegistry);

            clone.Name.Value = newName;
            clone.DisplayText = newName;
            clone.IsInEditMode = true;
            currentProject.Layouts.Add(clone);

            this.clonedLayout = clone;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            if (this.state.CurrentLayout == this.clonedLayout)
            {
                this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                    .Execute(
                        this.state.CurrentLayout =
                        this.state.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault(
                            l => !object.ReferenceEquals(this.clonedLayout, l)));
            }

            this.state.CurrentProject.InfomediaConfig.Layouts.Remove(this.clonedLayout);

            HistoryHelper.UnsetPredefinedFormulaReferences(this.clonedLayout, this.state);
            HistoryHelper.UnsetMediaReferences(this.clonedLayout, this.commandRegistry);
        }
    }
}
