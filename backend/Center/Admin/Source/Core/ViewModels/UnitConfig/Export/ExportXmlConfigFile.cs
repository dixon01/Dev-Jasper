// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportXmlConfigFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The export xml file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    using Gorba.Center.Admin.Core.Services;

    using ICSharpCode.AvalonEdit.Document;

    /// <summary>
    /// A generated (and editable) XML config file to be exported.
    /// </summary>
    public class ExportXmlConfigFile : ExportConfigFileBase
    {
        private TextDocument xmlDocument;

        private List<ErrorItem> errors;

        private XmlSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportXmlConfigFile"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="originalDocument">
        /// The original XML document contents. This should be an XML document with UTF-8 encoding.
        /// </param>
        public ExportXmlConfigFile(string fileName, string originalDocument)
            : base(fileName, originalDocument, "text/xml")
        {
            this.PropertyChanged += this.OnPropertyChanged;
            this.UpdateErrors();
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        public TextDocument XmlDocument
        {
            get
            {
                if (this.xmlDocument == null)
                {
                    this.xmlDocument = new TextDocument(this.Document);
                    this.xmlDocument.TextChanged += (s, e) => { this.Document = this.xmlDocument.Text; };
                }

                return this.xmlDocument;
            }
        }

        /// <summary>
        /// Gets or sets the XML schema against which this file should be verified.
        /// </summary>
        public XmlSchema Schema
        {
            get
            {
                return this.schema;
            }

            set
            {
                if (this.SetProperty(ref this.schema, value, () => this.Schema))
                {
                    this.UpdateErrors();
                }
            }
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
            if (string.IsNullOrEmpty(propertyName) || propertyName == "XmlDocument")
            {
                return this.errors;
            }

            return null;
        }

        private void UpdateErrors()
        {
            var hadErrors = this.errors != null && this.errors.Count > 0;
            if (this.Document == null)
            {
                this.errors = null;
            }
            else
            {
                this.errors =
                    XmlValidator.Validate(this.Document, this.Schema)
                        .Select(ex => new ErrorItem(ErrorState.Warning, this.Name + ": " + ex.Message))
                        .ToList();

                try
                {
                    // Loading the document to verify that it is well-formed
                    var bytes = Encoding.UTF8.GetBytes(this.Document);
                    using (var memoryStream = new MemoryStream(bytes))
                    {
                        var document = new XmlDocument();
                        document.Load(memoryStream);
                    }
                }
                catch (Exception)
                {
                    this.errors.Add(new ErrorItem(ErrorState.Error, this.Name + ": The document is not well formed"));
                }
            }

            var hasErrors = this.errors != null && this.errors.Count > 0;
            if (hadErrors != hasErrors)
            {
                this.RaisePropertyChanged(() => this.HasErrors);
            }

            if (hadErrors || hasErrors)
            {
                this.RaiseErrorsChanged(new DataErrorsChangedEventArgs("XmlDocument"));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Document")
            {
                this.UpdateErrors();
            }
        }
    }
}
