// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportPackageVersion.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportPackageVersion type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell.Software
{
    using System.Linq;
    using System.Management.Automation;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Software;

    /// <summary>
    /// Imports a package version.
    /// </summary>
    [Cmdlet(VerbsData.Import, "PackageVersion")]
    public sealed class ImportPackageVersion : ServiceCmdletBase
    {
        /// <summary>
        /// Gets or sets the package id.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the software version.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the structure.
        /// </summary>
        [Parameter(Mandatory = true)]
        public object Structure { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Parameter]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the version description.
        /// </summary>
        [Parameter]
        public string VersionDescription { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            Package package;
            using (var scope = this.CreateDataChannelScope<IPackageDataService>("Package"))
            {
                var query = PackageQuery.Create().WithPackageId(this.PackageId);
                package = scope.Channel.QueryAsync(query).Result.SingleOrDefault();
                if (package == null)
                {
                    package = new Package
                                   {
                                       Description = this.Description,
                                       PackageId = this.PackageId,
                                       ProductName = this.ProductName
                                   };
                    package = scope.Channel.AddAsync(package).Result;
                    this.WriteVerbose(string.Format("The package didn't exist. Created with id {0}", package.Id));
                }
                else
                {
                    this.WriteVerbose(string.Format("The package already exists with id {0}", package.Id));
                }
            }

            using (var scope = this.CreateDataChannelScope<IPackageVersionDataService>("PackageVersion"))
            {
                var query =
                    PackageVersionQuery.Create().WithPackage(package).WithSoftwareVersion(this.SoftwareVersion);
                var existing = scope.Channel.QueryAsync(query).Result.SingleOrDefault();
                if (existing != null)
                {
                    this.WriteVerbose("Package version already exists");
                    return;
                }

                var softwareVersion = new PackageVersion
                                          {
                                              Description = this.VersionDescription,
                                              Package = package,
                                              SoftwareVersion = this.SoftwareVersion,
                                              Structure = new XmlData(this.Structure)
                                          };
                var packageVersion = scope.Channel.AddAsync(softwareVersion).Result;
                this.WriteVerbose(string.Format("Package version added with id {0}", packageVersion.Id));
                this.WriteObject(packageVersion);
            }
        }
    }
}