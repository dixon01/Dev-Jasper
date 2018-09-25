// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportConfigFileBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportConfigFileBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    /// <summary>
    /// Base class for all (generated) config files to be exported.
    /// </summary>
    public abstract class ExportConfigFileBase : ExportFileBase, IExportableFile
    {
        private readonly string originalDocument;

        private string document;

        private bool hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConfigFileBase"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name without path.
        /// </param>
        /// <param name="originalDocument">
        /// The original config document as a string.
        /// </param>
        /// <param name="contentType">
        /// The MIME type of the config file.
        /// </param>
        protected ExportConfigFileBase(string fileName, string originalDocument, string contentType)
            : base(fileName)
        {
            this.originalDocument = originalDocument;
            this.Document = originalDocument;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets or sets the xml document.
        /// </summary>
        public string Document
        {
            get
            {
                return this.document;
            }

            set
            {
                this.SetProperty(ref this.document, value, () => this.Document);

                this.IsDirty = this.originalDocument != value;

                this.SetProperty(ref this.hasChanges, true, () => this.HasChanges);
            }
        }

        /// <summary>
        /// Gets the content type (MIME type) of this file.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this item or its children have changes.
        /// </summary>
        public override bool HasChanges
        {
            get
            {
                return this.hasChanges;
            }
        }

        string IExportableFile.DisplayName
        {
            get
            {
                return this.Name;
            }
        }

        /// <summary>
        /// Clears the <see cref="ExportItemBase.HasChanges"/> flag of this item and its children.
        /// </summary>
        public override void ClearHasChanges()
        {
            this.SetProperty(ref this.hasChanges, false, () => this.HasChanges);
        }
    }
}