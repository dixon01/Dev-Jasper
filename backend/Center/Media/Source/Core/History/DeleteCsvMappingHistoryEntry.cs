// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteCsvMappingHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a csv mapping element.
    /// </summary>
    public class DeleteCsvMappingHistoryEntry : HistoryEntryBase
    {
        private readonly CsvMappingDataViewModel mapping;

        private readonly MediaProjectDataViewModel project;

        private int listIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCsvMappingHistoryEntry"/> class.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public DeleteCsvMappingHistoryEntry(
            CsvMappingDataViewModel mapping,
            MediaProjectDataViewModel project,
            string displayText)
            : base(displayText)
        {
            this.mapping = mapping;
            this.project = project;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.listIndex = this.project.CsvMappings.IndexOf(this.mapping);
            this.project.CsvMappings.Remove(this.mapping);
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            if (this.listIndex > this.project.CsvMappings.Count - 1)
            {
                this.project.CsvMappings.Add(this.mapping);
            }
            else if (this.listIndex < 0)
            {
                this.project.CsvMappings.Insert(0, this.mapping);
            }
            else
            {
                this.project.CsvMappings.Insert(this.listIndex, this.mapping);
            }
        }
    }
}
