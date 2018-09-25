// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportResourceFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportResourceFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.Collections;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// A resource (e.g. from a software package version) to be exported.
    /// </summary>
    public class ExportResourceFile : ExportFileBase
    {
        private PackageVersionReadableModel source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportResourceFile"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="resource">
        /// The resource that represents the contents of this file.
        /// </param>
        public ExportResourceFile(string fileName, Resource resource)
            : base(fileName)
        {
            this.Resource = resource;
        }

        /// <summary>
        /// Gets the resource that represents the contents of this file.
        /// </summary>
        public Resource Resource { get; private set; }

        /// <summary>
        /// Gets or sets the package version from which this resource comes.
        /// This can be null if the resource did not originate from a package.
        /// </summary>
        public PackageVersionReadableModel Source
        {
            get
            {
                return this.source;
            }

            set
            {
                this.SetProperty(ref this.source, value, () => this.Source);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item or its children have changes.
        /// </summary>
        public override bool HasChanges
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the <see cref="ExportItemBase.HasChanges"/> flag of this item and its children.
        /// </summary>
        public override void ClearHasChanges()
        {
            // do nothing since a resource can't change
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>,
        /// to retrieve entity-level errors.
        /// </param>
        public override IEnumerable GetErrors(string propertyName)
        {
            return null;
        }
    }
}