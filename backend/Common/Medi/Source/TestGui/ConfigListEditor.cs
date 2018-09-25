// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigListEditor.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigListEditor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// UserControl showing the list of configurations.
    /// </summary>
    public partial class ConfigListEditor : UserControl
    {
        private int counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigListEditor"/> class.
        /// </summary>
        public ConfigListEditor()
        {
            this.InitializeComponent();

            this.buttonRemove.Enabled = false;
        }

        /// <summary>
        /// Gets or sets type of items that can be created with this list.
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets the node to which new items can be added.
        /// </summary>
        public TreeNode Node { get; set; }

        private void ListBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemove.Enabled = this.listBox1.SelectedItem != null;
        }

        private void ButtonRemoveClick(object sender, EventArgs e)
        {
            int index = this.listBox1.Items.IndexOf(this.listBox1.SelectedItem);
            this.listBox1.Items.RemoveAt(index);
            this.Node.Nodes.RemoveAt(index);
        }

        private void ButtonAddDropDownOpening(object sender, EventArgs e)
        {
            if (this.buttonAdd.HasDropDownItems)
            {
                return;
            }

            foreach (var typeInfo in TypeInfo.GetImplementations(this.ItemType))
            {
                var button = new ToolStripButton(typeInfo.Type.Name);
                button.Tag = typeInfo.Type;
                button.Click += this.ButtonOnClick;
                this.buttonAdd.DropDownItems.Add(button);
            }
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            var button = (ToolStripButton)sender;
            var type = button.Tag as Type;
            if (type == null)
            {
                return;
            }

            this.Node.Nodes.Add(TreeNodeFactory.CreateNode(type.Name, type));
            this.Node.Expand();
            this.listBox1.Items.Add(type.Name + this.counter++);
        }
    }
}
