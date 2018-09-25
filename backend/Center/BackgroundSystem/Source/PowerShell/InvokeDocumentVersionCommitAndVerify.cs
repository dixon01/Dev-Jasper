// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeDocumentVersionCommitAndVerify.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InvokeDocumentVersionCommitAndVerify type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;

    /// <summary>
    /// Invokes a commit on unit and verifies it.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "DocumentVersionCommitAndVerify")]
    public class InvokeDocumentVersionCommitAndVerify : AsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the change tracking managers.
        /// </summary>
        [Parameter(Mandatory = true)]
        public DocumentVersionChangeTrackingManager ChangeTrackingManager { get; set; }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        [Parameter(Mandatory = true)]
        [Alias("DocumentVersion")]
        public DocumentVersionWritableModel InputObject { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var unitReadableModel =
                await this.ChangeTrackingManager.CommitAndVerifyAsync(this.InputObject);
            this.WriteObject(unitReadableModel);
        }
    }
}