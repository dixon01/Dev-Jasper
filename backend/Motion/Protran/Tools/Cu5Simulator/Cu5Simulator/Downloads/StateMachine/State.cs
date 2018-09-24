// --------------------------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// A generic state for the download files state machine.
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Gets or sets NextStates.
        /// </summary>
        public List<string> NextStates { get; set; }

        /// <summary>
        /// Handles the things to do for this state.
        /// </summary>
        /// <param name="stateMachine">
        /// The state machine's stateMachine.
        /// </param>
        /// <param name="triplet">
        /// The triplet.
        /// </param>
        public abstract void Handle(Context stateMachine, Triplet triplet);
    }
}
