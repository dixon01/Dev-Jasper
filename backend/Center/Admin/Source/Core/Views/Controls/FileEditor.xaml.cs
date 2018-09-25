// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FileEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Controls
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;

    using Telerik.Windows.Documents.FormatProviders;
    using Telerik.Windows.Documents.FormatProviders.Txt;

    /// <summary>
    /// Interaction logic for FileEditor.xaml
    /// </summary>
    public partial class FileEditor
    {
        /// <summary>
        /// The file property.
        /// </summary>
        public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
            "File",
            typeof(ExportXmlConfigFile),
            typeof(FileEditor),
            new PropertyMetadata(default(ExportXmlConfigFile), OnFileChanged));

        private IDocumentFormatProvider currentFormatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEditor"/> class.
        /// </summary>
        public FileEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public ExportXmlConfigFile File
        {
            get
            {
                return (ExportXmlConfigFile)this.GetValue(FileProperty);
            }

            set
            {
                this.SetValue(FileProperty, value);
            }
        }

        private static void OnFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FileEditor)sender).UpdateDocument();
        }

        private void OnDocumentChanged(object sender, EventArgs e)
        {
            if (this.currentFormatProvider == null || this.File == null)
            {
                return;
            }

            var documentData = this.currentFormatProvider.Export(this.TextEditor.Document);
            this.File.Document = Encoding.UTF8.GetString(documentData);
        }

        private void UpdateDocument()
        {
            if (this.File == null)
            {
                return;
            }

            var fileExtension = Path.GetExtension(this.File.Name);
            this.currentFormatProvider = DocumentFormatProvidersManager.GetProviderByExtension(fileExtension);
            if (this.currentFormatProvider == null)
            {
                this.currentFormatProvider = new TxtFormatProvider();
            }

            var documentData = Encoding.UTF8.GetBytes(this.File.Document);
            this.TextEditor.Document = this.currentFormatProvider.Import(documentData);
        }

        private void OnDocumentContentChanged(object sender, EventArgs e)
        {
            if (this.currentFormatProvider != null)
            {
                var documentData = this.currentFormatProvider.Export(this.TextEditor.Document);
                this.File.Document = Encoding.UTF8.GetString(documentData);
            }
        }
    }
}
