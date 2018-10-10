// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ButtonInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ButtonInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// A button for the C74. It can be highlighted and consumes the <see cref="C74Keys.Ok"/> button
    /// to fire the <see cref="Pressed"/> event.
    /// </summary>
    public partial class ButtonInput : UserControl, IC74Input
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonInput"/> class.
        /// </summary>
        public ButtonInput()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is fired when the <see cref="C74Keys.Ok"/> button is pressed while focused.
        /// </summary>
        public event EventHandler Pressed;

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public ContentAlignment TextAlign
        {
            get
            {
                return this.label.TextAlign;
            }

            set
            {
                this.label.TextAlign = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.BackColor == SystemColors.ControlLight;
            }

            set
            {
                this.BackColor = value ? SystemColors.ControlLight : SystemColors.ControlDarkDark;
            }
        }

        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text
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
            if (key != C74Keys.Ok)
            {
                return false;
            }

            this.RaisePressed();
            return true;
        }

        /// <summary>
        /// Raises the <see cref="Pressed"/> event.
        /// </summary>
        protected virtual void RaisePressed()
        {
            var handler = this.Pressed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
