// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripletsProducedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Container of the triplets that must be sent to the TopBox.
    /// </summary>
    public class TripletsProducedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TripletsProducedEventArgs"/> class.
        /// </summary>
        public TripletsProducedEventArgs()
        {
            this.Triplets = new List<Triplet>();
        }

        /// <summary>
        /// Gets or sets the triplets to be sent to the TopBox.
        /// </summary>
        public List<Triplet> Triplets { get; set; }
    }
}
