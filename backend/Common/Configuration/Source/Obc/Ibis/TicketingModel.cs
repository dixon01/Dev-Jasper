// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicketingModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TicketingModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    /// <summary>
    /// The ticketing model.
    /// </summary>
    public enum TicketingModel
    {
        /// <summary>
        /// No ticketing is used in this bus.
        /// </summary>
        None,

        /// <summary>
        /// Krauth ticketing units are used in this bus.
        /// </summary>
        Krauth,

        /// <summary>
        /// Atron ticketing units are used in this bus.
        /// </summary>
        Atron
    }
}