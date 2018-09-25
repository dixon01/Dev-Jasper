// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityXmlEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for EntityXmlEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Widgets
{
    using System;
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Common.ServiceModel;

    using ICSharpCode.AvalonEdit.Document;

    /// <summary>
    /// Interaction logic for EntityXmlEditor.xaml
    /// </summary>
    public partial class EntityXmlEditor
    {
        /// <summary>
        /// The xml data property.
        /// </summary>
        public static readonly DependencyProperty XmlDataProperty = DependencyProperty.Register(
            "XmlData",
            typeof(XmlDataViewModel),
            typeof(EntityXmlEditor),
            new PropertyMetadata(default(XmlDataViewModel)));

        private Window xmlEditorWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityXmlEditor"/> class.
        /// </summary>
        public EntityXmlEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The popup opened.
        /// </summary>
        public event Action<EntityXmlEditor, Window> PopupOpened;

        /// <summary>
        /// Gets or sets the xml data.
        /// </summary>
        public XmlDataViewModel XmlData
        {
            get
            {
                return (XmlDataViewModel)this.GetValue(XmlDataProperty);
            }

            set
            {
                this.SetValue(XmlDataProperty, value);
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
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

        private void DisposeWindow()
        {
            ((XmlEditor)this.xmlEditorWindow.Content).SaveXml -= this.OnSaveEntityXml;
            this.xmlEditorWindow = null;
        }

        private void CreatePopup()
        {
            var xmlEditor = new XmlEditor
                                {
                                    XmlDocument = new TextDocument(this.XmlData.Xml ?? string.Empty),
                                    XmlSchema = this.XmlData.Schema
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

            this.OnPopupOpened(this.xmlEditorWindow);
        }

        private void OnSaveEntityXml(XmlEditor xmlEditor)
        {
            if (this.xmlEditorWindow != null)
            {
                xmlEditor.SaveXml -= this.OnSaveEntityXml;

                this.XmlData.Xml = xmlEditor.GetText();

                this.xmlEditorWindow.Close();
                this.DisposeWindow();
            }
        }

        private void OnPopupOpened(Window window)
        {
            var handler = this.PopupOpened;
            if (handler != null)
            {
                handler(this, window);
            }
        }
    }
}
