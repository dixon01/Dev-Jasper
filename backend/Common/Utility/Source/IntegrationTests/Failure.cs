// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Failure.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Failure type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.IntegrationTests
{
    using System;

    /// <summary>
    /// Defines a failure in an integration test.
    /// </summary>
    public class Failure
    {
        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public DateTime Time { get; set; }
    }
}