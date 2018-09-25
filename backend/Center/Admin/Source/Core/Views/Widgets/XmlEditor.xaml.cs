// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for XmlEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Widgets
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml.Schema;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.Services;

    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Folding;

    /// <summary>
    /// Interaction logic for XmlEditor.xaml
    /// </summary>
    public partial class XmlEditor
    {
        /// <summary>
        /// The xml document property.
        /// </summary>
        public static readonly DependencyProperty XmlDocumentProperty = DependencyProperty.Register(
            "XmlDocument",
            typeof(TextDocument),
            typeof(XmlEditor),
            new PropertyMetadata(default(TextDocument), XmlDocumentChanged));

        /// <summary>
        /// The xml document property.
        /// </summary>
        public static readonly DependencyProperty XmlSchemaProperty = DependencyProperty.Register(
            "XmlSchema",
            typeof(XmlSchema),
            typeof(XmlEditor),
            new PropertyMetadata(default(XmlSchema), XmlSchemaChanged));

        /// <summary>
        /// The is dirty property.
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(XmlEditor),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// The show save button property.
        /// </summary>
        public static readonly DependencyProperty ShowSaveButtonProperty = DependencyProperty.Register(
            "ShowSaveButton",
            typeof(bool),
            typeof(XmlEditor),
            new PropertyMetadata(true));

        /// <summary>
        /// The show save button property.
        /// </summary>
        public static readonly DependencyProperty CanSaveProperty = DependencyProperty.Register(
            "CanSave",
            typeof(bool),
            typeof(XmlEditor),
            new PropertyMetadata(true));

        private string originalData;

        private XmlFoldingStrategy foldingStrategy;

        private FoldingManager foldingManager;

        private TextMarkerService textMarkerService;

        private ToolTip toolTip;

        private BitmapImage imageOk;

        private BitmapImage imageError;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEditor"/> class.
        /// </summary>
        public XmlEditor()
        {
            this.InitializeComponent();

            this.imageOk =
                new BitmapImage(
                    new Uri(
                        @"pack://application:,,,/Gorba.Center.Admin.Core;"
                        + "component/Resources/Icons/config_error_ok_16x16.png"));

            this.imageError =
                new BitmapImage(
                    new Uri(
                        @"pack://application:,,,/Gorba.Center.Admin.Core;"
                        + "component/Resources/Icons/config_error_error_16x16.png"));

            var textView = this.TextEditor.TextArea.TextView;
            textView.MouseHover += this.TextViewOnMouseHover;
            textView.MouseHoverStopped += this.TextViewOnMouseHoverStopped;
            textView.VisualLinesChanged += (s, e) => this.CloseToolTip();

            this.UpdateCanSave();
        }

        /// <summary>
        /// The save xml.
        /// </summary>
        public event Action<XmlEditor> SaveXml;

        /// <summary>
        /// Gets or sets the xml document.
        /// </summary>
        public TextDocument XmlDocument
        {
            get
            {
                return (TextDocument)GetValue(XmlDocumentProperty);
            }

            set
            {
                SetValue(XmlDocumentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the xml schema.
        /// </summary>
        public XmlSchema XmlSchema
        {
            get
            {
                return (XmlSchema)GetValue(XmlSchemaProperty);
            }

            set
            {
                SetValue(XmlSchemaProperty, value);
                this.UpdateCanSave();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is dirty.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return (bool)GetValue(IsDirtyProperty);
            }

            set
            {
                SetValue(IsDirtyProperty, value);
                this.UpdateCanSave();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show save button.
        /// </summary>
        public bool ShowSaveButton
        {
            get
            {
                return (bool)GetValue(ShowSaveButtonProperty);
            }

            set
            {
                SetValue(ShowSaveButtonProperty, value);
                this.UpdateCanSave();
            }
        }

        /// <summary>
        /// Gets a value indicating whether we can save the XML.
        /// </summary>
        public bool CanSave
        {
            get
            {
                return (bool)GetValue(CanSaveProperty);
            }

            private set
            {
                SetValue(CanSaveProperty, value);
            }
        }

        /// <summary>
        /// The get text.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetText()
        {
            return this.XmlDocument == null ? null : this.XmlDocument.GetText(0, this.XmlDocument.TextLength);
        }

        /// <summary>
        /// The on save xml.
        /// </summary>
        protected virtual void RaiseSaveXml()
        {
            var handler = this.SaveXml;
            if (handler != null)
            {
                handler(this);
            }
        }

        private static void XmlDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as XmlEditor;
            if (editor != null)
            {
                editor.OnXmlDocumentChanged(e.NewValue as TextDocument);
            }
        }

        private static void XmlSchemaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as XmlEditor;
            if (editor != null)
            {
                editor.OnXmlSchemaChanged();
            }
        }

        private void OnXmlDocumentChanged(TextDocument xmlDocument)
        {
            var textView = this.TextEditor.TextArea.TextView;

            if (this.foldingManager != null)
            {
                FoldingManager.Uninstall(this.foldingManager);
                this.foldingManager = null;
                this.foldingStrategy = null;
            }

            if (this.textMarkerService != null)
            {
                textView.BackgroundRenderers.Remove(this.textMarkerService);
                textView.LineTransformers.Remove(this.textMarkerService);
                textView.Services.RemoveService(typeof(TextMarkerService));
                this.textMarkerService = null;
            }

            if (xmlDocument == null)
            {
                this.originalData = null;
                return;
            }

            this.TextEditor.Document = xmlDocument;

            this.foldingManager = FoldingManager.Install(this.TextEditor.TextArea);
            this.foldingStrategy = new XmlFoldingStrategy();
            this.foldingStrategy.UpdateFoldings(this.foldingManager, this.TextEditor.Document);

            this.textMarkerService = new TextMarkerService(this.TextEditor);
            textView.BackgroundRenderers.Add(this.textMarkerService);
            textView.LineTransformers.Add(this.textMarkerService);
            textView.Services.AddService(typeof(TextMarkerService), this.textMarkerService);

            this.originalData = xmlDocument.GetText(0, xmlDocument.TextLength);

            this.ValidateXml(xmlDocument.GetText(0, xmlDocument.TextLength));
        }

        private void OnXmlSchemaChanged()
        {
            this.ValidateXml(this.GetText());
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (this.XmlDocument == null)
            {
                return;
            }

            if (this.foldingStrategy != null)
            {
                this.foldingStrategy.UpdateFoldings(this.foldingManager, this.TextEditor.Document);
            }

            if (this.originalData == null)
            {
                return;
            }

            var text = this.GetText() ?? string.Empty;
            this.IsDirty = this.originalData != text;

            this.ValidateXml(text);
        }

        private void ValidateXml(string text)
        {
            if (this.textMarkerService == null)
            {
                return;
            }

            this.textMarkerService.Clear();
            var exceptions = XmlValidator.Validate(text, this.XmlSchema);
            foreach (var exception in exceptions)
            {
                this.AddValidationError(exception.Message, exception.LinePosition, exception.LineNumber);
            }

            if (exceptions.Length == 1)
            {
                this.SetError(
                    this.imageError,
                    string.Format(AdminStrings.XmlEditor_OneError_Format, exceptions[0].LineNumber));
            }
            else if (exceptions.Length > 1)
            {
                var lineNumbers = string.Join(
                    ", ",
                    exceptions.Reverse()
                        .Skip(1)
                        .Reverse()
                        .Select(e => e.LineNumber.ToString(CultureInfo.InvariantCulture)));
                var message = string.Format(
                    AdminStrings.XmlEditor_Errors_Format,
                    exceptions.Length,
                    lineNumbers,
                    exceptions.Last().LineNumber);
                this.SetError(this.imageError, message);
            }
            else if (this.XmlSchema == null)
            {
                this.SetError(this.imageOk, AdminStrings.XmlEditor_OK_NoSchema);
            }
            else
            {
                this.SetError(this.imageOk, AdminStrings.XmlEditor_OK_WithSchema);
            }
        }

        private void SetError(ImageSource image, string text)
        {
            this.ErrorImage.Source = image;
            this.ErrorText.Text = text;
            this.UpdateCanSave();
        }

        private void AddValidationError(string message, int linePosition, int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > this.TextEditor.Document.LineCount)
            {
                return;
            }

            int offset = this.TextEditor.Document.GetOffset(new TextLocation(lineNumber, linePosition));
            int endOffset = TextUtilities.GetNextCaretPosition(
                this.TextEditor.Document,
                offset,
                System.Windows.Documents.LogicalDirection.Forward,
                CaretPositioningMode.WordBorderOrSymbol);
            if (endOffset < 0)
            {
                endOffset = this.TextEditor.Document.TextLength;
            }

            int length = endOffset - offset;
            if (length < 2)
            {
                length = Math.Min(2, this.TextEditor.Document.TextLength - offset);
            }

            this.textMarkerService.Create(offset, length, message);
        }

        private void UpdateCanSave()
        {
            this.CanSave = this.ShowSaveButton && this.IsDirty && this.imageOk.Equals(this.ErrorImage.Source);
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            this.RaiseSaveXml();
        }

        private void OnTextEditorLoaded(object sender, RoutedEventArgs e)
        {
            this.TextEditor.Focus();
        }

        private void TextViewOnMouseHover(object sender, MouseEventArgs e)
        {
            if (this.TextEditor.Document == null)
            {
                return;
            }

            var textView = this.TextEditor.TextArea.TextView;
            var pos = textView.GetPositionFloor(e.GetPosition(textView) + textView.ScrollOffset);
            if (!pos.HasValue)
            {
                return;
            }

            int offset = this.TextEditor.Document.GetOffset(pos.Value.Location);

            var markersAtOffset = this.textMarkerService.GetMarkersAtOffset(offset);
            var markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

            if (markerWithToolTip == null || this.toolTip != null)
            {
                return;
            }

            this.toolTip = new ToolTip();
            this.toolTip.Closed += (s, ev) => this.toolTip = null;
            this.toolTip.PlacementTarget = this;
            this.toolTip.Content = new TextBlock
                                       {
                                           Text = markerWithToolTip.ToolTip,
                                           TextWrapping = TextWrapping.Wrap
                                       };
            this.toolTip.IsOpen = true;
            e.Handled = true;
        }

        private void TextViewOnMouseHoverStopped(object sender, MouseEventArgs e)
        {
            this.CloseToolTip();
            e.Handled = true;
        }

        private void CloseToolTip()
        {
            if (this.toolTip != null)
            {
                this.toolTip.IsOpen = false;
            }
        }
    }
}
