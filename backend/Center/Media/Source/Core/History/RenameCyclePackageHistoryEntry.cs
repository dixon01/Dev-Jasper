// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenameCyclePackageHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the renaming of a cycle package.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    /// <summary>
    /// History entry that contains all information to undo / redo the renaming of a cycle package.
    /// </summary>
    public class RenameCyclePackageHistoryEntry : HistoryEntryBase
    {
         private readonly CyclePackageConfigDataViewModel element;
        private readonly string newName;

        private string oldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameCyclePackageHistoryEntry"/> class.
        /// </summary>
        /// <param name="element"> The element to be renamed </param>
        /// <param name="newName"> The new name. </param>
        /// <param name="displayText"> The text to be displayed for this Entry on the UI. </param>
        public RenameCyclePackageHistoryEntry(
            CyclePackageConfigDataViewModel element,
            string newName,
            string displayText)
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
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.element.MakeDirty();

            this.element.Name.Value = this.oldName;
            this.element.DisplayText = this.oldName;
        }
    }
}
