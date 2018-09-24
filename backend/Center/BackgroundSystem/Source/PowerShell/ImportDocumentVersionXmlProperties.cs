// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportDocumentVersionXmlProperties.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportDocumentVersionXmlProperties type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;

    /// <summary>
    /// Loads the Xml on a document version.
    /// </summary>
    [Cmdlet(VerbsData.Import, "DocumentVersionXmlProperties")]
    public class ImportDocumentVersionXmlProperties : AsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the input object.
        /// </summary>
        [Parameter(Mandatory = true)]
        public DocumentVersionReadableModel InputObject { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            await this.InputObject.LoadXmlPropertiesAsync();
            this.WriteObject(this.InputObject);
        }
    }
}