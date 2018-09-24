// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerStackConfigEditor.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PeerStackConfigEditor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// The peer stack config editor.
    /// </summary>
    public partial class PeerStackConfigEditor : UserControl
    {
        private PeerConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerStackConfigEditor"/> class.
        /// </summary>
        public PeerStackConfigEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the config.
        /// </summary>
        public PeerConfig Config
        {
            get
            {
                return this.config;
            }

            set
            {
                if (this.config == value)
                {
                    return;
                }

                this.config = value;

                this.PopulateComboBox(this.comboBox1, typeof(CodecConfig));
                this.PopulateComboBox(
                    this.comboBox2,
                    this.Config.GetType().GetProperty("Transport").PropertyType);

                this.propertyGrid1.SelectedObject = this.config;
            }
        }

        /// <summary>
        /// Gets or sets the tree node associated with this config.
        /// </summary>
        public TreeNode Node { get; set; }

        private void PopulateComboBox(ComboBox comboBox, Type baseType)
        {
            comboBox.Items.Clear();

            var types = TypeInfo.GetImplementations(baseType);
            comboBox.Items.AddRange(types.Cast<object>().ToArray());
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = this.comboBox1.SelectedItem as TypeInfo;
            if (type == null)
            {
                return;
            }

            var codecConfigProperty = this.config.GetType().GetProperty("Codec");
            var oldValue = codecConfigProperty.GetValue(this.config, null);
            if (oldValue != null && oldValue.GetType() == type.Type)
            {
                return;
            }

            codecConfigProperty.SetValue(
                this.Config,
                Activator.CreateInstance(type.Type),
                null);
            this.propertyGrid1.SelectedObject = this.Config;
        }

        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = this.comboBox2.SelectedItem as TypeInfo;
            if (type == null)
            {
                return;
            }

            var transportConfigProperty = this.Config.GetType().GetProperty("Transport");
            var oldValue = transportConfigProperty.GetValue(this.config, null);
            if (oldValue != null && oldValue.GetType() == type.Type)
            {
                return;
            }

            transportConfigProperty.SetValue(
                this.Config,
                Activator.CreateInstance(type.Type),
                null);
            this.propertyGrid1.SelectedObject = this.Config;
        }
    }
}
