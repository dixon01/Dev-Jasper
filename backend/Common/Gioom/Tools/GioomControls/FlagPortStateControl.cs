// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagPortStateControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FlagPortStateControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// The flag port state control.
    /// </summary>
    public partial class FlagPortStateControl : PortStateControlBase
    {
        private bool updatingValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagPortStateControl"/> class.
        /// </summary>
        public FlagPortStateControl()
        {
            this.InitializeComponent();
            this.UpdateText();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is read-only.
        /// </summary>
        protected override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }

            set
            {
                base.ReadOnly = value;
                this.checkBoxValue.Enabled = !value;
            }
        }

        /// <summary>
        /// Updates this control with the given value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        protected override void UpdateValue(IOValue value)
        {
            base.UpdateValue(value);

            this.updatingValue = true;
            try
            {
                this.checkBoxValue.Checked = value.Value != 0;
                this.UpdateText();
            }
            finally
            {
                this.updatingValue = false;
            }
        }

        private void CheckBoxValueOnCheckedChanged(object sender, EventArgs e)
        {
            if (this.updatingValue || this.Port == null)
            {
                return;
            }

            this.Port.Value = this.checkBoxValue.Checked ? FlagValues.True : FlagValues.False;
            this.UpdateText();
        }

        private void UpdateText()
        {
            this.checkBoxValue.Text = this.checkBoxValue.Checked ? "On" : "Off";
        }
    }
}
