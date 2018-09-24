// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdminApplicationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Runtime.Serialization;

    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// The admin application state.
    /// </summary>
    [Export]
    [Export(typeof(IApplicationState))]
    [Export(typeof(IAdminApplicationState))]
    [DataContract]
    public class AdminApplicationState : ConnectedApplicationState, IAdminApplicationState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminApplicationState"/> class.
        /// </summary>
        public AdminApplicationState()
        {
            this.RecentlyEditedEntities = new Dictionary<string, IList<RecentlyEditedEntityReference>>();
            this.Stages = new List<StageModel>();
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IAdminShell Shell { get; private set; }

        /// <summary>
        /// Gets or sets the recently edited entities.
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<RecentlyEditedEntityReference>> RecentlyEditedEntities { get; set; }

        /// <summary>
        /// Gets or sets the list of all stages with their properties.
        /// </summary>
        [DataMember]
        public List<StageModel> Stages { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="shell">The shell.</param>
        public void Initialize(IAdminShell shell)
        {
            this.Shell = shell;
        }
    }
}