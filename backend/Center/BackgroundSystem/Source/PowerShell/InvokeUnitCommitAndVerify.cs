// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeUnitCommitAndVerify.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InvokeUnitCommitAndVerify type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;

    /// <summary>
    /// Invokes a commit on unit and verifies it.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "UnitCommitAndVerify")]
    public class InvokeUnitCommitAndVerify : AsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the change tracking managers.
        /// </summary>
        [Parameter(Mandatory = true)]
        public ChangeTrackingManagersSet ChangeTrackingManagers { get; set; }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        [Parameter(Mandatory = true)]
        public UnitWritableModel Unit { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var unitReadableModel =
                await this.ChangeTrackingManagers.UnitChangeTrackingManager.CommitAndVerifyAsync(this.Unit);
            this.WriteObject(unitReadableModel);
        }
    }
}