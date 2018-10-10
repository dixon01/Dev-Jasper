// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationPartControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for TransformationPartControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig.Parts
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.Core.Views.Widgets;
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Transformations;

    using ICSharpCode.AvalonEdit.Document;

    using Telerik.Windows.Controls.Data.PropertyGrid;

    /// <summary>
    /// Interaction logic for TransformationPartControl.xaml
    /// </summary>
    public partial class TransformationPartControl : UserControl
    {
        private Window xmlEditorWindow;

        private XmlSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationPartControl"/> class.
        /// </summary>
        public TransformationPartControl()
        {
            this.InitializeComponent();

            this.IsVisibleChanged += (s, e) => this.CloseWindow();
            this.DataContextChanged += (s, e) => this.CloseWindow();
        }

        private TransformationPartViewModel ViewModel
        {
            get
            {
                return (TransformationPartViewModel)this.DataContext;
            }
        }

        private XmlSchema Schema
        {
            get
            {
                if (this.schema == null)
                {
                    var original = IbisConfig.Schema;
                    original.Items.Add(new XmlSchemaElement
                    {
                        Name = "Chain",
                        SchemaTypeName = new XmlQualifiedName("TransformationChain")
                    });
                    this.schema = original;
                }

                return this.schema;
            }
        }

        private void DisposeWindow()
        {
            ((XmlEditor)this.xmlEditorWindow.Content).SaveXml -= this.OnSaveEntityXml;
            this.xmlEditorWindow = null;
        }

        private void CreatePopup()
        {
            var xmlEditor = new XmlEditor
                                {
                                    XmlDocument = new TextDocument(this.CreateXmlText()),
                                    XmlSchema = this.Schema
                                };
            this.xmlEditorWindow = new Window
                                       {
                                           Content = xmlEditor,
                                           Width = 400,
                                           Height = 600,
                                           WindowStyle = WindowStyle.ToolWindow,
                                           Owner = Window.GetWindow(this),
                                           WindowStartupLocation = WindowStartupLocation.CenterOwner
                                       };
            this.xmlEditorWindow.Show();

            ((XmlEditor)this.xmlEditorWindow.Content).SaveXml += this.OnSaveEntityXml;
        }

        private string CreateXmlText()
        {
            var chain = new Chain { Id = this.ViewModel.Editor.Id };
            foreach (var transformation in this.ViewModel.Editor.Transformations)
            {
                chain.Transformations.Add(transformation.CreateConfig());
            }

            try
            {
                var serializer = new XmlSerializer(typeof(Chain));
                var writer = new StringWriter();
                var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
                var xmlWriter = XmlWriter.Create(writer, settings);
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                serializer.Serialize(xmlWriter, chain, namespaces);

                return writer.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private void SaveXmlText(string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Chain));
                var reader = new StringReader(xml);
                var chain = (Chain)serializer.Deserialize(reader);
                var editor = this.ViewModel.Editor;
                editor.Id = chain.Id;
                editor.Transformations.Clear();
                foreach (var transformation in chain.Transformations.Where(t => !(t is Integer)))
                {
                    editor.Transformations.Add(TransformationDataViewModelBase.Create(transformation));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadPropertyGridOnValidated(object sender, PropertyGridValidatedEventArgs e)
        {
            ((TransformationPartViewModel)this.DataContext).MakeDirty();
        }

        private void AddItemButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.DropDownButton.IsOpen = false;
        }

        private void EditXmlButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (this.xmlEditorWindow != null && !this.xmlEditorWindow.IsVisible)
            {
                this.DisposeWindow();
            }

            if (this.xmlEditorWindow == null)
            {
                this.CreatePopup();
            }
            else
            {
                this.xmlEditorWindow.BringIntoView();
            }
        }

        private void OnSaveEntityXml(XmlEditor xmlEditor)
        {
            if (this.xmlEditorWindow == null)
            {
                return;
            }

            xmlEditor.SaveXml -= this.OnSaveEntityXml;

            this.SaveXmlText(xmlEditor.GetText());

            this.CloseWindow();
        }

        private void CloseWindow()
        {
            if (this.xmlEditorWindow == null)
            {
                return;
            }

            this.xmlEditorWindow.Close();
            this.DisposeWindow();
        }

        private void PropertyGridOnAutoGeneratingPropertyDefinition(
            object sender, AutoGeneratingPropertyDefinitionEventArgs e)
        {
            var propertyName = e.PropertyDefinition.SourceProperty.Name;
            if (propertyName == "ClonedFrom"
                || propertyName == "DisplayName"
                || propertyName == "HasErrors")
            {
                // internal properties are never shown
                e.Cancel = true;
            }
        }
    }
}
