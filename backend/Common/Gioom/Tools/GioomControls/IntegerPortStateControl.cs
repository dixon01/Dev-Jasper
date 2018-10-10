// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerPortStateControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerPortStateControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// The integer port state control.
    /// </summary>
    public partial class IntegerPortStateControl : PortStateControlBase
    {
        private bool updatingValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerPortStateControl"/> class.
        /// </summary>
        public IntegerPortStateControl()
        {
            this.InitializeComponent();
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
                this.numericUpDown.Enabled = !value;
            }
        }

        /// <summary>
        /// Changes the currently used port to a new one.
        /// </summary>
        /// <param name="newPort">
        /// The new port.
        /// </param>
        protected override void ChangePort(IPort newPort)
        {
            var values = (IntegerValues)newPort.Info.ValidValues;
            this.updatingValue = true;
            try
            {
                this.numericUpDown.Minimum = values.MinValue;
                this.numericUpDown.Maximum = values.MaxValue;
            }
            finally
            {
                this.updatingValue = false;
            }

            base.ChangePort(newPort);
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
                this.numericUpDown.Value = value.Value;
            }
            finally
            {
                this.updatingValue = false;
            }
        }

        private void NumericUpDownOnValueChanged(object sender, EventArgs e)
        {
            if (this.updatingValue || this.Port == null)
            {
                return;
            }

            this.Port.IntegerValue = (int)this.numericUpDown.Value;
        }
    }
}
