// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageBoxControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;

    /// <summary>
    /// Message box that can be put on top of a control.
    /// </summary>
    public partial class MessageBoxControl : PopupControl, IC74Input
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxControl"/> class.
        /// </summary>
        public MessageBoxControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is fired whenever the "OK" button is pressed on the message box.
        /// </summary>
        public event EventHandler OkPressed;

        /// <summary>
        /// Gets or sets a value indicating whether this control is selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the message shown.
        /// </summary>
        public string Message
        {
            get
            {
                return this.labelMessage.Text;
            }

            set
            {
                this.labelMessage.Text = value;
            }
        }

        /// <summary>
        /// Processes the given key.
        /// </summary>
        /// <param name="key">
        /// The key. This is never <see cref="C74Keys.None"/>.
        /// </param>
        /// <returns>
        /// True if the key was handled otherwise false.
        /// </returns>
        public bool ProcessKey(C74Keys key)
        {
            return this.buttonOk.ProcessKey(key);
        }

        /// <summary>
        /// Raises the <see cref="OkPressed"/> event.
        /// </summary>
        protected virtual void RaiseOkPressed()
        {
            var handler = this.OkPressed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void ButtonOkOnPressed(object sender, EventArgs e)
        {
            this.RaiseOkPressed();
        }
    }
}
