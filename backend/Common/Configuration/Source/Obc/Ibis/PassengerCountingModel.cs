// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PassengerCountingModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PassengerCountingModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    /// <summary>
    /// The passenger counting model.
    /// </summary>
    public enum PassengerCountingModel
    {
        /// <summary>
        /// No passenger counting is used in this bus.
        /// </summary>
        None,

        /// <summary>
        /// The iris passenger counting is used.
        /// </summary>
        Iris
    }
}