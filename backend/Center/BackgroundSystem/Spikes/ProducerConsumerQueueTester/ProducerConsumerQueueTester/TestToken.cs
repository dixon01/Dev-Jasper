// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestToken.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    /// <summary>
    /// Defines an object used for testing purposes.
    /// </summary>
    public class TestToken
    {
        /// <summary>
        /// Gets or sets the time when the object was created.
        /// </summary>
        /// <value>
        /// The time when the object was created.
        /// </value>
        public long Time { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TestToken: {0}", this.Time);
        }
    }
}