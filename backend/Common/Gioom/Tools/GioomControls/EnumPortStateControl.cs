// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumPortStateControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumPortStateControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System;
    using System.Globalization;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// The enum port state control.
    /// </summary>
    public partial class EnumPortStateControl : PortStateControlBase
    {
        private bool updatingValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumPortStateControl"/> class.
        /// </summary>
        public EnumPortStateControl()
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
                this.comboBox.Enabled = !value;
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
            var values = (EnumValues)newPort.Info.ValidValues;
            this.updatingValue = true;
            try
            {
                this.textBoxValue.Clear();
                this.comboBox.Items.Clear();

                foreach (var value in values.Values)
                {
                    this.comboBox.Items.Add(value);
                }
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
                this.textBoxValue.Text = value.Value.ToString(CultureInfo.InvariantCulture);
                foreach (IOValue item in this.comboBox.Items)
                {
                    if (item.Value == value.Value)
                    {
                        this.comboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            finally
            {
                this.updatingValue = false;
            }
        }

        private void ComboBoxOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var value = this.comboBox.SelectedItem as IOValue;
            if (this.updatingValue || this.Port == null || value == null)
            {
                return;
            }

            this.Port.Value = value;
            this.textBoxValue.Text = value.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
