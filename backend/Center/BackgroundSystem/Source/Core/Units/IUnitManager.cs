// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUnitManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Units
{
    using Gorba.Center.BackgroundSystem.Core.Dynamic;

    /// <summary>
    /// Defines the interface for the unit manager.
    /// The unit manager supervises the lifetime of unit controllers and updates the connection state of units.
    /// </summary>
    public interface IUnitManager
    {
        /// <summary>
        /// Sends a live update to all units assigned to the specified update group.
        /// </summary>
        /// <param name="queuedMessage">
        /// The live update to send.
        /// </param>
        void SendLiveUpdate(QueuedMessage queuedMessage);
    }
}