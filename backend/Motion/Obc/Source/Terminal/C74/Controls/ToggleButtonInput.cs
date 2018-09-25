// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleButtonInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ToggleButtonInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System.Drawing;

    /// <summary>
    /// A toggle button for the C74. It can be highlighted and checked.
    /// It consumes the <see cref="C74Keys.Ok"/> button.
    /// </summary>
    public partial class ToggleButtonInput : ButtonInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButtonInput"/> class.
        /// </summary>
        public ToggleButtonInput()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is checked (toggle on).
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.label.ForeColor == Color.Black;
            }

            set
            {
                if (value)
                {
                    this.label.ForeColor = Color.Black;
                    this.label.BackColor = Color.White;
                    this.label.Font = new Font(this.label.Font, FontStyle.Bold);
                }
                else
                {
                    this.label.ForeColor = Color.White;
                    this.label.BackColor = SystemColors.ControlDarkDark;
                    this.label.Font = new Font(this.label.Font, FontStyle.Regular);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ButtonInput.Pressed"/> event.
        /// </summary>
        protected override void RaisePressed()
        {
            this.IsChecked = !this.IsChecked;
            base.RaisePressed();
        }
    }
}
