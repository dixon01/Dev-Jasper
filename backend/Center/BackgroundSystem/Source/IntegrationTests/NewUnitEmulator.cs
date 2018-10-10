// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewUnitEmulator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NewUnitEmulator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests
{
    using System.Management.Automation;

    using Gorba.Common.Utility.IntegrationTests;

    /// <summary>
    /// Writes a new <see cref="UnitEmulator"/> instance to the pipeline.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "UnitEmulator")]
    public class NewUnitEmulator : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        [Parameter(Mandatory = true)]
        public IntegrationTestContext TestContext { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            var emulator = new UnitEmulator(this.TestContext);
            this.WriteObject(emulator);
        }
    }
}