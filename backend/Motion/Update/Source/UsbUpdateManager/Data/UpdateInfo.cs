// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Information about a single update don on a specific unit.
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateInfo"/> class.
        /// </summary>
        public UpdateInfo()
        {
            this.States = new List<UpdateStateInfo>();
        }

        /// <summary>
        /// Gets or sets the human readable name of the update.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the update command of the update.
        /// </summary>
        public UpdateCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the list of states that were reported to this tool.
        /// </summary>
        [XmlElement("StateChange")]
        public List<UpdateStateInfo> States { get; set; }

        /// <summary>
        /// Gets the currently valid state or null if the state is unknown.
        /// </summary>
        public UpdateStateInfo CurrentState
        {
            get
            {
                UpdateStateInfo latestState = null;

                foreach (var state in this.States)
                {
                    if (latestState == null || state.TimeStamp > latestState.TimeStamp)
                    {
                        latestState = state;
                    }
                }

                return latestState;
            }
        }
    }
}