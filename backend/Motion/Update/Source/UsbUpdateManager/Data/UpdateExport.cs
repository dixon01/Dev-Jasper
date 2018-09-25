// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExport.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateExport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// The update export contains all resources and commands needed to update
    /// all units in a project to match the current project structure.
    /// </summary>
    public class UpdateExport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExport"/> class.
        /// </summary>
        public UpdateExport()
        {
            this.Commands = new List<UpdateCommand>();
            this.Resources = new List<ResourceInfo>();
        }

        /// <summary>
        /// Gets the update commands to be executed by the units.
        /// </summary>
        public List<UpdateCommand> Commands { get; private set; }

        /// <summary>
        /// Gets the resources required resources to execute the <see cref="Commands"/>.
        /// </summary>
        public List<ResourceInfo> Resources { get; private set; }
    }
}