// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateAgentPersistence.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Persistence
{
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Persistence configuration for Update agent
    /// </summary>
    public class UpdateAgentPersistence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAgentPersistence"/> class.
        /// </summary>
        public UpdateAgentPersistence()
        {
            this.BackgroundId = string.Empty;
            this.ParkedUpdateCommands = new List<UpdateCommand>();
        }

        /// <summary>
        /// Gets or sets the last successful update's Background system GUID.
        /// </summary>
        public string BackgroundId { get; set; }

        /// <summary>
        /// Gets or sets the last successful update's update index for the above Background system GUID.
        /// </summary>
        public int UpdateIndex { get; set; }

        /// <summary>
        /// Gets or sets the list of parked update commands to be persisted.
        /// </summary>
        public List<UpdateCommand> ParkedUpdateCommands { get; set; }
    }
}
