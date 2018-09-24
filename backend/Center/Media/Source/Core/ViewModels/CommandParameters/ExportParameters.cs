// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the set of parameters required to export a project to the server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the set of parameters required to export a project to the server.
    /// </summary>
    public class ExportParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportParameters"/> class.
        /// </summary>
        /// <param name="start">
        /// The start date.
        /// </param>
        /// <param name="end">
        /// The end date.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="updateGroups">
        /// The groups that should receive the update.
        /// </param>
        public ExportParameters(
            DateTime start,
            DateTime end,
            string description,
            IEnumerable<UpdateGroupItemViewModel> updateGroups)
        {
            this.Start = start;
            this.End = end;
            this.Description = description;
            this.UpdateGroups = updateGroups;
        }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        public DateTime End { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the selected update groups.
        /// </summary>
        public IEnumerable<UpdateGroupItemViewModel> UpdateGroups { get; private set; }
    }
}
