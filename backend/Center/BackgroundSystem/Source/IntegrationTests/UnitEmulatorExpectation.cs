// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitEmulatorExpectation.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitEmulatorExpectation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests
{
    /// <summary>
    /// The unit emulator expectation.
    /// </summary>
    public class UnitEmulatorExpectation
    {
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically send transferred state feedback.
        /// </summary>
        public bool SendTransferred { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically send transferring state feedback.
        /// </summary>
        public bool SendTransferring { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send installed state feedback.
        /// </summary>
        public bool SendInstalled { get; set; }
    }
}