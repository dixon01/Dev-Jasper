// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextInputDialog.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextInputDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Dialog that prompts the user to input a string.
    /// </summary>
    public partial class TextInputDialog : Form
    {
        private bool inputRequired;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputDialog"/> class.
        /// </summary>
        public TextInputDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the label shown above the text input field.
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
        /// Gets or sets the input text shown in the text input field.
        /// </summary>
        [DefaultValue("")]
        public string InputText
        {
            get
            {
                return this.textBox.Text;
            }

            set
            {
                this.textBox.Text = value;
                this.ValidateInput();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any input required.
        /// If this property is set to true, the "OK" button is only
        /// enabled if the text input field contains at least one character.
        /// </summary>
        [DefaultValue(false)]
        public bool InputRequired
        {
            get
            {
                return this.inputRequired;
            }

            set
            {
                if (value == this.inputRequired)
                {
                    return;
                }

                this.inputRequired = value;
                this.ValidateInput();
            }
        }

        private void ValidateInput()
        {
            this.buttonOk.Enabled = this.InputText.Length > 0;
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            this.ValidateInput();
        }
    }
}
