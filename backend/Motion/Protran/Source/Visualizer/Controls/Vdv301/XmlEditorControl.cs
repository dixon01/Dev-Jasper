// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlEditorControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlEditorControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using NLog;

    using ScintillaNET;

    /// <summary>
    /// The XML editor control.
    /// </summary>
    public partial class XmlEditorControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Scintilla editor;

        private string xmlText;

        private bool isReadOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEditorControl"/> class.
        /// </summary>
        public XmlEditorControl()
        {
            this.InitializeComponent();

            try
            {
                this.editor = new Scintilla();
                this.editor.ConfigurationManager.Language = "xml";
                this.editor.Margins[0].Width = 30;
                this.editor.Dock = DockStyle.Fill;
                this.editor.Font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
                this.Controls.Add(this.editor);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Couldn't load Scintilla");
            }
        }

        /// <summary>
        /// Gets or sets the XML text of the editor.
        /// </summary>
        public string XmlText
        {
            get
            {
                return this.editor == null ? this.xmlText : this.editor.Text;
            }

            set
            {
                this.xmlText = value;
                if (this.editor == null)
                {
                    return;
                }

                this.editor.IsReadOnly = false;
                this.editor.Text = value;
                this.editor.IsReadOnly = this.isReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this editor is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.editor == null ? this.isReadOnly : this.editor.IsReadOnly;
            }

            set
            {
                this.isReadOnly = value;
                if (this.editor == null)
                {
                    return;
                }

                this.editor.IsReadOnly = value;
            }
        }
    }
}
