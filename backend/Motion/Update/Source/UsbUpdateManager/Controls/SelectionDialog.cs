// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionDialog.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectionDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Dialog for selecting items with a combo box.
    /// </summary>
    public partial class SelectionDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDialog"/> class.
        /// </summary>
        public SelectionDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the label shown above the combo box.
        /// </summary>
        [DefaultValue("")]
        public string Label
        {
            get
            {
                return this.label.Text;
            }

            set
            {
                this.label.Text = value;
            }
        }

        /// <summary>
        /// Gets the items in the combo box.
        /// </summary>
        public ComboBox.ObjectCollection Items
        {
            get
            {
                return this.comboBox.Items;
            }
        }

        /// <summary>
        /// Gets or sets the selected item in the combo box.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return this.comboBox.SelectedItem;
            }

            set
            {
                this.comboBox.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected index in the combo box.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.comboBox.SelectedIndex;
            }

            set
            {
                this.comboBox.SelectedIndex = value;
            }
        }

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonOk.Enabled = this.comboBox.SelectedIndex >= 0;
        }
    }
}
