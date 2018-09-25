// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCsvMappingHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a csv mapping element.
    /// </summary>
    public class CreateCsvMappingHistoryEntry : HistoryEntryBase
    {
        private readonly CsvMappingDataViewModel mapping;

        private readonly MediaProjectDataViewModel project;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCsvMappingHistoryEntry"/> class.
        /// </summary>
        /// <param name="mapping">
        /// The mapping data view model.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public CreateCsvMappingHistoryEntry(
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
            this.project.CsvMappings.Add(this.mapping);
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.project.CsvMappings.Remove(this.mapping);
        }
    }
}
